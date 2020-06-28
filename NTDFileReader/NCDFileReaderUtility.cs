// Credits: This code was written after reading JR Stokka's solution at
// https://github.com/jrstokka/NinjaTraderNCDFiles/blob/master/NCDFile.cs

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

        public static IEnumerable<NCDFileTick> Read(Stream stream) {
            using var br = new BinaryReader(stream);
            br.ReadUInt32();
            var incrementSize = br.ReadDouble();
            var price = br.ReadDouble();
            var timeTicks = br.ReadInt64();
            while (stream.Position < stream.Length) {

                /********************************************************************************
                * byte1 Format: ppsssttt
                *   pp  = price flags
                *   sss = spread (bid/offer) flags
                *   ttt = time flags 
                *   
                * byte2 format: vvvppppp
                *   vvv  = volume flags
                *   ppppp = price delta or unused, depending on price flags in Byte1
                ********************************************************************************/

                var byte1 = br.ReadByte();
                var byte2 = br.ReadByte();

                timeTicks += (byte1 & 0b111) switch
                {
                    0b000 => 0,
                    0b001 => br.ReadByte(),
                    0b010 => br.ReadBigEndianLong(2),
                    0b011 => br.ReadBigEndianLong(4),
                    0b100 => br.ReadBigEndianLong(8),
                    0b101 => br.ReadByte() * TimeSpan.TicksPerSecond,
                    _ => throw new Exception($"Unknown time flag"),
                };

                price = price.Increment(incrementSize, (byte1 >> 6) switch
                {
                    0b00 => 0,
                    0b01 => (byte2 & 0b11111) - (1 << 4),
                    0b10 => br.ReadByte() - (1 << 7),
                    0b11 => (int)(br.ReadBigEndianUInt(4) - (1 << 31)), // not happy with this expression
                    _ => throw new Exception("Unknown price flag"),
                });

                var spreadFlags = (byte1 >> 3) & 0b111;
                var bidOffset = spreadFlags & 0b001; // 1 or 0
                var askOffset = 1 - bidOffset;       // 0 or 1
                switch (spreadFlags) {

                    case 0b110:
                        var x = br.ReadByte();
                        bidOffset = x >> 4;
                        askOffset = x & 0b1111;
                        break;

                    case 0b111:
                        bidOffset = br.ReadByte();
                        askOffset = br.ReadByte();
                        break;

                    default:
                        if ((spreadFlags & 0b010) > 0) {
                            bidOffset *= 2;
                            askOffset *= 2;
                        } else if ((spreadFlags & 0b100) > 0) {
                            bidOffset *= 3;
                            askOffset *= 3;
                        }
                        break;
                }
                var bid = price.Increment(incrementSize, -bidOffset);
                var offer = price.Increment(incrementSize, askOffset);

                var volume = (byte2 >> 5) switch
                {
                    0b001 => br.ReadByte(),
                    0b010 => 100L * br.ReadByte(),
                    0b011 => 500L * br.ReadByte(),
                    0b100 => 1000L * br.ReadByte(),
                    0b101 => br.ReadBigEndianLong(2),
                    0b110 => br.ReadBigEndianLong(4),
                    0b111 => br.ReadBigEndianLong(8),
                    _ => throw new Exception("Unknown volume flag.")
                };

                yield return new NCDFileTick(bid, offer, price, volume, new DateTime(timeTicks, DateTimeKind.Local));
            }
        }

        static double Increment(this double value, double increment, int numIncrements) {
            if (numIncrements == 0) return value;
            return (double)((decimal)increment * ((int)Math.Round(value / increment, MidpointRounding.AwayFromZero) + numIncrements));
        }
    }
}
 