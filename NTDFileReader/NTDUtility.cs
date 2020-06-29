using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTDFileReader {

    /// <summary>
    /// Reads the ticks from a NinjaTrader 7 .ntd historical tick data file.
    /// If you're using this, remember that the ticks are stored in the same timezone as your computer.
    /// If you change the timezone on your computer's clock after NinjaTrader has downloaded data, the data files will contain wrong timestamps.
    /// Created with thanks to the following resources:
    /// https://www.bigmiketrading.com/ninjatrader-programming/7396-ntd-file-specification.html
    /// https://www.bigmiketrading.com/elite-circle/6802-gomrecorder-2-a-37.html#post112912
    /// </summary>
    public static class NTDUtility {

        public static IEnumerable<NTDTick> ReadTicks(Stream stream) {

            using var br = new BinaryReader(stream);

            br.BaseStream.Seek(0, SeekOrigin.Begin);
            var priceIncrement = -br.ReadDouble();

            br.BaseStream.Seek(0xC, SeekOrigin.Begin);
            var recordCount = br.ReadUInt32();

            br.BaseStream.Seek(0x10, SeekOrigin.Begin);
            var price = br.ReadDouble();

            br.BaseStream.Seek(0x30, SeekOrigin.Begin);
            var time = new DateTime(br.ReadInt64());

            br.BaseStream.Seek(0x38, SeekOrigin.Begin);
            var volume = br.ReadUInt64();

            yield return new NTDTick(time, price, volume);

            br.BaseStream.Seek(0x40, SeekOrigin.Begin);

            for (uint i = 1; i < recordCount; i++) {

                var mask = br.ReadByte();

                time = time.AddSeconds((mask & 0b11) switch
                {
                    0b00 => 0.0,
                    0b01 => br.ReadBigEndianUInt(1),
                    0b10 => br.ReadBigEndianUInt(2),
                    0b11 => br.ReadBigEndianUInt(3),
                    _ => throw new NotImplementedException("Unexpected mask value for time"),
                });

                price = price.Increment(priceIncrement, (((mask >> 2) & 0b11) switch
                {
                    0b00 => 0,
                    0b01 => br.ReadBigEndianInt(1) - (1 << 7),
                    0b10 => br.ReadBigEndianInt(2) - (1 << 15),
                    0b11 => br.ReadBigEndianInt(4) - (1 << 31),
                    _ => throw new NotImplementedException("Unexpected mask value for price"),
                }));

                volume = (((mask >> 4) & 0b111) switch
                {
                    0b001 => br.ReadBigEndianULong(1),
                    0b110 => br.ReadBigEndianULong(2),
                    0b111 => br.ReadBigEndianULong(4),
                    0b010 => br.ReadBigEndianULong(8),
                    0b011 => br.ReadBigEndianULong(1) * 100,
                    0b100 => br.ReadBigEndianULong(1) * 500,
                    0b101 => br.ReadBigEndianULong(1) * 1000,
                    _ => throw new NotImplementedException("Unexpected mask value for volume"),
                });

                yield return new NTDTick(time, price, volume);
            }
        }
    }
}
