using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTDFileReader
{
    /// <summary>
    /// Reads the ticks from a NinjaTrader .ntd historical tick data file.
    /// If you're using this, remember that the ticks are stored in the same timezone as your computer.
    /// If you change the timezone on your computer's clock after NinjaTrader has downloaded data, the data files will contain wrong timestamps.
    /// Created with thanks to the following resources:
    /// https://www.bigmiketrading.com/ninjatrader-programming/7396-ntd-file-specification.html
    /// https://www.bigmiketrading.com/elite-circle/6802-gomrecorder-2-a-37.html#post112912
    /// </summary>
    public static class NTDFileReaderUtility
    {
        const int b000 = 0;
        const int b001 = 1;
        const int b010 = 2;
        const int b011 = 3;
        const int b100 = 4;
        const int b101 = 5;
        const int b110 = 6;
        const int b111 = 7;

        public static IEnumerable<NTDFileTick> Read(string filename) {
            return Read(File.ReadAllBytes(filename));
        }

        public static IEnumerable<NTDFileTick> Read(byte[] bytes) {

            using (var ms = new MemoryStream(bytes)) {
                using (var br = new BinaryReader(ms)) {

                    decimal price;
                    DateTime time;
                    ulong volume;

                    br.BaseStream.Seek(0, SeekOrigin.Begin);
                    var priceMultiplier = (decimal)-br.ReadDouble();

                    br.BaseStream.Seek(0xC, SeekOrigin.Begin);
                    var recordCount = br.ReadUInt32();

                    br.BaseStream.Seek(0x10, SeekOrigin.Begin);
                    price = (decimal)br.ReadDouble();

                    br.BaseStream.Seek(0x30, SeekOrigin.Begin);
                    var timeTicks = br.ReadInt64();
                    time = new DateTime(timeTicks);

                    br.BaseStream.Seek(0x38, SeekOrigin.Begin);
                    volume = (uint)br.ReadUInt64();

                    yield return new NTDFileTick {
                        Price = price,
                        TimeStamp = time,
                        Volume = volume,
                    };

                    br.BaseStream.Seek(0x40, SeekOrigin.Begin);

                    for (uint i = 1; i < recordCount; i++) {

                        var mask = br.ReadByte();
                        uint deltaTime;
                        int deltaPrice;

                        // time
                        switch (mask & b011) { // 00000011
                            case b000: deltaTime = 0; break;
                            case b001: deltaTime = br.ReadBigEndianUInt(1); break;
                            case b010: deltaTime = br.ReadBigEndianUInt(2); break;
                            case b011: deltaTime = br.ReadBigEndianUInt(3); break;
                            default: throw new NotImplementedException("Unexpected mask value for time");
                        }
                        time = time.AddSeconds(deltaTime);

                        // price
                        switch ((mask >> 2) & b011) { // 00001100
                            case b000: deltaPrice = 0; break;
                            case b001: deltaPrice = br.GetBigEndianInt(1) - 0x80; break;
                            case b010: deltaPrice = br.GetBigEndianInt(2) - 0x4000; break;
                            case b011: deltaPrice = br.GetBigEndianInt(4) - 0x40000000; break;
                            default: throw new NotImplementedException("Unexpected mask value for price");
                        }
                        price += priceMultiplier * deltaPrice;

                        // volume
                        switch ((mask >> 4) & b111) { // 01110000
                            case b001: volume = br.ReadBigEndianUInt(1); break;
                            case b110: volume = br.ReadBigEndianUInt(2); break;
                            case b111: volume = br.ReadBigEndianUInt(4); break;
                            case b010: volume = br.ReadBigEndianUInt(8); break;
                            case b011: volume = br.ReadBigEndianUInt(1) * 100; break;
                            case b100: volume = br.ReadBigEndianUInt(1) * 500; break;
                            case b101: volume = br.ReadBigEndianUInt(1) * 1000; break;
                            default: throw new NotImplementedException("Unexpected mask value for volume");
                        }

                        yield return new NTDFileTick {
                            TimeStamp = time,
                            Price = price,
                            Volume = volume
                        };
                    }
                }
            }
        }
    }
}
