using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace NTDFileReader {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static class NCDFileReaderUtility {

        #region class TimeFlags
        class TimeFlags {

            /// <summary>
            /// Time does not change.
            /// </summary>
            public const byte Nochange = 0b000;

            /// <summary>
            /// Time increases by the number of ticks contained in the next byte.
            /// </summary>
            public const byte OneByteTicks = 0b001;

            /// <summary>
            /// Time increases by the number of ticks contained in the next two bytes read as a big-endian long.
            /// </summary>
            public const byte TwoByteTicks = 0b010;

            /// <summary>
            /// Time increases by the number of ticks contained in the next four bytes read as a big-endian long.
            /// </summary>
            public const byte FourByteTicks = 0b011;

            /// <summary>
            /// Time increases by the number of ticks contained in the next eight bytes read as a big-endian long.
            /// </summary>
            public const byte EightByteTicks = 0b100;

            /// <summary>
            /// Time increases by the number of seconds contained in the next byte.
            /// </summary>
            public const byte OneByteSeconds = 0b101;
        }
        #endregion
        #region class PriceFlags
        class PriceFlags {

            /// <summary>
            /// Price does not change.
            /// </summary>
            public const byte NoChange = 0b00;

            /// <summary>
            /// Price change is contained in the last four bits of Byte2.
            /// </summary>
            public const byte InsideByte2 = 0b01;

            /// <summary>
            /// Price change is contained in the next one byte.
            /// </summary>
            public const byte OneByteDelta = 0b10;

            /// <summary>
            /// Price change is contained in the next four bytes read as a BigEndian int.
            /// </summary>
            public const byte FourByteDelta = 0b11;

        }
        #endregion
        #region class SpreadFlags
        class SpreadFlags {
            public const byte BidTrade = 0b000;
            public const byte AskTrade = 0b001;
            public const byte DefaultSpreadX2 = 0b010;
            public const byte DefaultSpreadX3 = 0b100;
            public const byte OneByte = 0b110;
            public const byte TwoBytes = 0b111;
        }
        #endregion
        #region class VolumeFlags
        class VolumeFlags {
            public const byte Volume1 = 0b001;
            public const byte Volume1x100 = 0b010;
            public const byte Volume1x500 = 0b011;
            public const byte Volume1x1000 = 0b100;
            public const byte Volume2 = 0b101;
            public const byte Volume4 = 0b110;
            public const byte Volume8 = 0b111;
        }
        #endregion

        public static IEnumerable<NCDFileTick> Read(Stream stream) {
            using (var br = new BinaryReader(stream)) {
                var tickSizeTime = br.ReadUInt32();
                var priceIncrement = br.ReadDouble();
                var price = br.ReadDouble();
                var ticks = (long)br.ReadUInt64();
                while (stream.Position < stream.Length) {

                    /// Byte1 Format: ppsssttt
                    ///   pp  = price flags
                    ///   sss = spread (bid/offer) flags
                    ///   ttt = time flags <see cref="TimeFlags"/>
                    ///   
                    /// Byte2 format: vvv.pppp
                    ///   vvv  = volume flags
                    ///   .    = unknown
                    ///   pppp = price delta or unused, depending on price flags in Byte1

                    #region done 

                    var byte1 = br.ReadByte();
                    var byte2 = br.ReadByte();

                    var timeFlags = byte1 & 0b111;
                    var ticksDelta = timeFlags switch
                    {
                        TimeFlags.Nochange => 0,
                        TimeFlags.OneByteTicks => br.ReadByte(),
                        TimeFlags.TwoByteTicks => br.ReadBigEndianLong(2),
                        TimeFlags.FourByteTicks => br.ReadBigEndianLong(4),
                        TimeFlags.EightByteTicks => br.ReadBigEndianLong(8),
                        TimeFlags.OneByteSeconds => br.ReadByte() * TimeSpan.TicksPerSecond,
                        _ => throw new Exception($"Unrecognized TimeFlag in Byte1"),
                    };
                    ticks += ticksDelta;

                    var priceFlags = byte1 >> 6;
                    var priceOffset = priceFlags switch
                    {
                        PriceFlags.NoChange => 0,
                        PriceFlags.InsideByte2 => ((byte2 & 0b1111) - 0b1000),
                        PriceFlags.OneByteDelta => br.ReadByte() - (1 << 7),
                        PriceFlags.FourByteDelta => (int)(br.ReadBigEndianUInt(4) - (1 << 31)),
                        _ => throw new Exception("Unrecognized PriceFlag in Byte1"),
                    };
                    price = price.Increment(priceIncrement, priceOffset);

                    var bidOffset = 0;
                    var askOffset = 0;
                    var spreadFlags = (byte1 >> 3) & 0b111;
                    switch (spreadFlags) {

                        case SpreadFlags.TwoBytes:
                            bidOffset = br.ReadByte();
                            askOffset = br.ReadByte();
                            break;

                        case SpreadFlags.OneByte:
                            var x = br.ReadByte();
                            bidOffset = x >> 4;
                            askOffset = x & 0b1111;
                            break;

                        default:
                            bidOffset = spreadFlags & 0b001; // 1 or 0
                            askOffset = 1 - bidOffset;       // 0 or 1
                            if ((spreadFlags & 0b010) > 0) {
                                bidOffset *= 2;
                                askOffset *= 2;
                            } else if ((spreadFlags & 0b100) > 0) {
                                bidOffset *= 3;
                                askOffset *= 3;
                            }
                            break;
                    }
                    var bid = price.Increment(priceIncrement, -bidOffset);
                    var ask = price.Increment(priceIncrement, askOffset);

                    #endregion

                    var volumeFlags = byte2 >> 5;
                    var volume = volumeFlags switch
                    {
                        VolumeFlags.Volume1 => br.ReadByte(),
                        VolumeFlags.Volume1x100 => 100L * br.ReadByte(),
                        VolumeFlags.Volume1x500 => 500L * br.ReadByte(),
                        VolumeFlags.Volume1x1000 => 1000L * br.ReadByte(),
                        VolumeFlags.Volume2 => br.ReadBigEndianLong(2),
                        VolumeFlags.Volume4 => br.ReadBigEndianLong(4),
                        VolumeFlags.Volume8 => br.ReadBigEndianLong(8),
                        _ => throw new Exception("Unknown volume flag.")
                    };

                    yield return new NCDFileTick(bid, ask, price, volume, new DateTime(ticks, DateTimeKind.Local).ToUniversalTime());
                }
            }
        }

        public static double Increment(this double value, double increment, int numIncrements) {
            if (numIncrements == 0) return value;
            return (double)((decimal)increment * ((int)Math.Round(value / increment, MidpointRounding.AwayFromZero) + numIncrements));
        }
    }
}