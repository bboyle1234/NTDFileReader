using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NTDFileReader {

    public static class flagextensions {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasOption(this byte mask, byte checkflag) {
            return (mask & checkflag) == checkflag;
        }

        public static double AddTicks(this double value, double tickSize, int ticks) {
            return 0;
        }
    }

    public class Mask1 {
        public const byte NoTime = 0;                     // x.....000    
        public const byte Time1 = 1;                      // x.....001
        public const byte Time2 = 2;                      // x.....010                          
        public const byte Time4 = 3;                      // x.....011
        public const byte Time8 = 4;                      // x.....100
        public const byte Time1Secs = 5;                  // x.....101
        public const byte Ask = 8;                        // x..xx1...  
        public const byte DefaultSpreadX2 = 16;           // x..x1x...  
        public const byte DefaultSpreadX3 = 32;           // x..1xx...  
        public const byte Spread1 = 48;                   // x..110...  
        public const byte Spread2 = 56;                   // x..111...
        public const byte PriceEmbeddedInMask2 = 64;      // x01......
        public const byte Price1 = 128;                   // x10......
        public const byte Price4 = 192;                   // x11......
    }

    public class Mask2 {
        public const byte PriceDeltaBit1 = 1;             // x....0001
        public const byte PriceDeltaBit2 = 2;             // x....0010
        public const byte PriceDeltaBit3 = 4;             // x....0100
        public const byte PriceDeltaBit4 = 8;             // x....1000
        public const byte Volume1 = 32;                   // x001.....
        public const byte Volume1X100 = 64;               // x010.....
        public const byte Volume1X500 = 96;               // x011..... 
        public const byte Volume1X1000 = 128;             // x100.....
        public const byte Volume2 = 160;                  // x101.....
        public const byte Volume4 = 192;                  // x110.....
        public const byte Volume8 = 224;                  // x111.....
    }

    /// <summary>
    /// </summary>
    public static class NCDFileReaderUtility {

        public static IEnumerable<NCDFileTick> Read(Stream stream) {
            using (var br = new BinaryReader(stream)) {
                var tickSizeTime = br.ReadUInt32();
                var tickSize = br.ReadDouble();
                var price = br.ReadDouble();
                var ticks = (long)br.ReadUInt64();
                while (stream.Position < stream.Length) {

                    /// Byte1 Format: ppsssttt
                    ///   pp  = price flags
                    ///   sss = spread (bid/offer) flags
                    ///   ttt = time flags
                    ///   
                    /// Byte2 format: vvv.pppp
                    ///   vvv  = volume flags
                    ///   .    = unknown
                    ///   pppp = price flags, or price value, depending on price flags in Byte1

                    var byte1 = br.ReadByte();
                    var byte2 = br.ReadByte();

                    var timeFlags = byte1 & 0b111;
                    const byte Nochange = 0b000; 
                    const byte OneByteTicks = 0b001;
                    const byte TwoByteTicks = 0b010;
                    const byte FourByteTicks = 0b011;
                    const byte EightByteTicks = 0b100;
                    const byte OneByteSeconds = 0b101;
                    if (timeFlags == Nochange) { 
                    } else if (timeFlags == OneByteTicks) { 
                        ticks += br.ReadByte();
                    } else if (timeFlags == TwoByteTicks) { 
                        ticks += br.ReadBigEndianLong(2);
                    } else if (timeFlags == FourByteTicks) {
                        ticks += br.ReadBigEndianLong(4);
                    } else if (timeFlags == EightByteTicks) {
                        ticks += br.ReadBigEndianLong(8);
                    } else if (timeFlags == OneByteSeconds) {
                        ticks += br.ReadByte() * TimeSpan.TicksPerSecond;
                    } else {
                        throw new Exception($"Unrecognized Time Flag in Byte1");
                    }

                    var priceTicksDiff = 0;
                    var priceMask = byte1 >> 6;
                    if (priceMask == 0b10) {
                        priceTicksDiff = br.ReadByte() - (1 << 7);
                    } else if (priceMask == 0b11) {
                        priceTicksDiff = (int)(br.ReadBigEndianUInt(4) - (1 << 31));
                    } else if (priceMask == 0b01) {

                    }
                    price = price.AddTicks(tickSize, priceTicksDiff);
                }
            }
        }

        [Flags]
        public enum NCDFileType {
            Tick = 1,
            Minute = 2,
            Day = 3
        }


        [Flags]
        public enum MinuteMask1Flags {
            NoTime = 0,
            Time1 = 1,
            Time2 = 2,
            Time4 = 3,
            Open1 = 4,
            Open2 = 8,
            Open4 = 12,
            Volume1 = 32,
            Volume1X100 = 64,
            Volume1X500 = 96,
            Volume1X1000 = 128,
            Volume2 = 160,
            Volume4 = 192,
            Volume8 = 240
        }

        [Flags]
        public enum MinuteMask2Flags {
            Close1 = 1,
            Close2 = 2,
            Close4 = 3,
            High1 = 16,
            High2 = 32,
            High4 = 48,
            Low1 = 64,
            Low2 = 128,
            Low4 = 192
        }
        public static class GlobalOptions {
            public static string HistoricalDataPath;
        }
        public interface INCDRecord { }
        public struct MinuteRecord : INCDRecord {
            public DateTime DateTime;
            public double Open;
            public double High;
            public double Low;
            public double Close;
            public long Volume;
            public MinuteRecord(DateTime dt, double open, double high, double low, double close, long volume) {
                DateTime = dt;
                Open = open;
                Close = close;
                High = high;
                Low = low;
                Volume = volume;
            }
        }
        public struct TickRecord : INCDRecord {
            public DateTime DateTime;
            public double Price;
            public double Bid;
            public double Ask;
            public long Volume;
            public TickRecord(DateTime dt, double price, double bid, double ask, long volume) {
                DateTime = dt;
                Price = price;
                Bid = bid;
                Ask = ask;
                Volume = volume;
            }
        }

        /// <summary>
        /// Helper Class to process multiple NCDFiles such as for an instrument and a date or for a date range.
        /// </summary>
        public class NCDFiles {
            public NCDFiles(NCDFileType ncdFileType, string instrumentId, DateTime date, int numberDaysForward = 0, int numberDaysBack = 0) {
                _NCDFileType = ncdFileType;
                _instrumentId = instrumentId;
                // build file list
                DateTime startDate = date.Date.AddDays(-numberDaysBack);
                DateTime endDate = date.Date.AddDays(numberDaysForward);

                string instrumentDataPath = null;
                if (_NCDFileType == NCDFileType.Minute)
                    instrumentDataPath = Path.Combine(NinjaTrader.GlobalOptions.HistoricalDataPath, "minute", instrumentId);
                else if (_NCDFileType == NCDFileType.Tick)
                    instrumentDataPath = Path.Combine(NinjaTrader.GlobalOptions.HistoricalDataPath, "tick", instrumentId);
                else
                    throw new Exception($"Invalid NCDFileType: {_NCDFileType}");

                _files = new List<string>(
                    Directory.GetFiles(instrumentDataPath).Where(filePath =>
                    (DateTime.ParseExact(Path.GetFileName(filePath).Substring(0, 8), "yyyyMMdd", CultureInfo.InvariantCulture)
                    .Between(startDate, endDate))).OrderBy(filePath => filePath)
                    );

                if (_files.Count == 0) {
                    if (startDate == endDate)
                        throw new Exception($"No files found for date: {date:MM/dd/yyyy}.");
                    else
                        throw new Exception($"No files found for date range: {startDate:MM/dd/yyyy} - {endDate:MM/dd/yyyy}.");
                }
                _filesLeft = new List<string>(_files);

                GetNextFile();
            }

            NCDFileType _NCDFileType;
            string _instrumentId;

            List<string> _files;
            List<string> _filesLeft;

            INCDFile _currentFile;

            public INCDFile CurrentFile { get { return _currentFile; } }
            public NCDFileType NCDFileType { get { return _NCDFileType; } }
            void GetNextFile() {

                if (_NCDFileType == NCDFileType.Tick)
                    _currentFile = new NCDTickFile(_filesLeft[0]);
                else if (_NCDFileType == NCDFileType.Minute)
                    _currentFile = new NCDMinuteFile(_filesLeft[0]);

                _filesLeft.RemoveAt(0);
            }

            public INCDRecord ReadNextRecord() {
                if (!EndOfData) {
                    if (_currentFile.EndOfFile)
                        GetNextFile();
                    return _currentFile.ReadNextRecord();
                } else
                    throw new Exception("No more data.  Must safe check for EndOfData before reading next record.");
            }

            public bool EndOfData { get { return (_currentFile.EndOfFile && _IsLastFile); } }

            bool _IsLastFile { get { return (_filesLeft.Count == 0); } }
        }

        public class NCDTickFile : NCDFile<TickRecord> {
            public NCDTickFile(string path) : base(NCDFileType.Tick, path) {
                _defaultSpread = _tickSizePrice;
            }
            public override INCDRecord ReadNextRecord() {
                return ReadNextTickRecord();
            }

            Mask1 _lastMask1;
            Mask2 _lastMask2;
            Double _lastBid;
            Double _lastAsk;
            Double _defaultSpread;
            private TickRecord ReadNextTickRecord() {
                Mask1 mask1 = (Mask1)_br.ReadByte();
                Mask2 mask2 = (Mask2)_br.ReadByte();

                // Inputs
                UInt64 timeDelta;
                uint priceDelta = 0;
                UInt64 volumeDelta;
                uint priceDeltaOffset = 0;

                // Outputs
                DateTime barDateTime;
                Double barPrice;
                Double barBid;
                Double barAsk;
                long barVolume;

                //
                // check the Time flag
                //
                if (mask1.HasOption(Mask1.Time1Secs)) {
                    timeDelta = _br.ReadByte();
                    barDateTime = _lastDateTime.AddTicks((long)(timeDelta * 10000000));
                } else if (mask1.HasOption(Mask1.Time8)) {
                    long bigTimeDelta = (long)_br.ReadUInt64BE();
                    barDateTime = _lastDateTime.AddTicks(bigTimeDelta);
                } else {
                    if (mask1.HasOption(Mask1.Time4))
                        timeDelta = /*(int)*/_br.ReadUInt32BE();
                    else if (mask1.HasOption(Mask1.Time2))
                        timeDelta = _br.ReadUInt16BE();
                    else if (mask1.HasOption(Mask1.Time1))
                        timeDelta = _br.ReadByte();
                    else if (mask1.HasOption(Mask1.NoTime))
                        timeDelta = 0;
                    else
                        throw new Exception($"Unrecognized Time Flag in Mask1: {Convert.ToString((short)mask1, 2)}\n" +
                            $"Mask2: {Convert.ToString((short)mask2, 2)}\n" +
                            $"FileName: {FileName}\n" +
                            $"Position: {_br.BaseStream.Position}");
                    barDateTime = _lastDateTime.AddTicks((long)timeDelta);
                }
                //
                // check the Price flag
                //
                // There are two price markers in the top two high order bits in Mask1.
                // If either is set then a price must be calculated because it's not the same as the previous bar otherwise there is no price
                // change and the price is the same as the previous bar.
                //
                // There are three options...
                //
                // (1) If only the first bit is set then the Price Delta must be read as a single byte following the Time Delta if applicable and
                // an offset of 128 applied.
                //
                // b10xxxxxx
                //
                // (2) If both are set the Price Delta must be read as a 4 byte unsigned integer following the Time Delta if applicable and
                // an offset of 0x80000000 applied.
                //
                // b11xxxxxx
                //
                // (3) If only the 2nd bit is set then the Price Delta is embedded in the 4 low order bits of Mask2.
                //
                // If a value exists within the low 4 bits of Mask2 then you may use Mask 2 as the Price Delta.
                //
                // b01xxxxxx
                //
                // Mask2 - bxxxxpppp 
                // Price delta = b0000pppp
                // Price delta Offset = bxxxx0000
                // 

                if (mask1.HasOption(Mask1.Price4)) {
                    priceDelta = _br.ReadUInt32BE();
                    priceDeltaOffset = 2147483648; // 0x80000000
                } else if (mask1.HasOption(Mask1.Price1)) {
                    priceDelta = _br.ReadByte();
                    priceDeltaOffset = 128; // 0x80
                } else if (mask1.HasOption(Mask1.PriceEmbeddedInMask2)) {
                    if (mask2.HasOption(Mask2.PriceDeltaBit1) || mask2.HasOption(Mask2.PriceDeltaBit2) || mask2.HasOption(Mask2.PriceDeltaBit3) || mask2.HasOption(Mask2.PriceDeltaBit4)) {
                        priceDelta = (byte)mask2;
                        if (mask2.HasOption(Mask2.Volume8))
                            priceDeltaOffset = 240;
                        else if (mask2.HasOption(Mask2.Volume4))
                            priceDeltaOffset = 208;
                        else if (mask2.HasOption(Mask2.Volume2))
                            priceDeltaOffset = 176;
                        else if (mask2.HasOption(Mask2.Volume1X1000))
                            priceDeltaOffset = 142;
                        else if (mask2.HasOption(Mask2.Volume1X500))
                            priceDeltaOffset = 112;
                        else if (mask2.HasOption(Mask2.Volume1))
                            priceDeltaOffset = 48;
                        else if (mask2.HasOption(Mask2.Volume1X100))
                            priceDeltaOffset = 80; // x30
                        else
                            throw new Exception($"Unknown Volume Flag in Mask2: {Convert.ToString((short)mask2, 2)}\n" +
                                $"Mask1: {Convert.ToString((short)mask1, 2)}\n" +
                                $"FileName: {FileName}\n" +
                                $"Position: {_br.BaseStream.Position}");
                    } else
                        throw new Exception($"No expected Price Delta embedded in Mask2: {Convert.ToString((short)mask2, 2)}\n" +
                            $"Mask1: {Convert.ToString((short)mask1, 2)}\n" +
                            $"FileName: {FileName}\n" +
                            $"Position: {_br.BaseStream.Position}");
                } else if (mask2.HasOption(Mask2.PriceDeltaBit1) || mask2.HasOption(Mask2.PriceDeltaBit2) || mask2.HasOption(Mask2.PriceDeltaBit3) || mask2.HasOption(Mask2.PriceDeltaBit4)) {
                    throw new Exception($"Price Delta embedded in Mask2 where none expected: {Convert.ToString((short)mask2, 2)}\n" +
                        $"Mask1: {Convert.ToString((short)mask1, 2)}\n" +
                        $"FileName: {FileName}\n" +
                        $"Position: {_br.BaseStream.Position}");
                }
                barPrice = Math.Round(_lastPrice + ((int)(priceDelta - priceDeltaOffset) * _tickSizePrice), 2);

                //
                // check the Bid/Ask flag
                //
                int spreadMultiplier;
                bool bidTrade;

                if (mask1.HasOption(Mask1.Spread2)) {
                    spreadMultiplier = _br.ReadUInt16BE();
                    if (spreadMultiplier < 256) {
                        bidTrade = true;
                    } else {
                        bidTrade = false;
                        spreadMultiplier /= 256;
                    }
                } else if (mask1.HasOption(Mask1.Spread1)) {
                    spreadMultiplier = _br.ReadByte();
                    if (spreadMultiplier < 16) {
                        bidTrade = true;
                    } else {
                        bidTrade = false;
                        spreadMultiplier /= 16;
                    }
                } else {
                    if (mask1.HasOption(Mask1.DefaultSpreadX3))
                        spreadMultiplier = 3;
                    else if (mask1.HasOption(Mask1.DefaultSpreadX2))
                        spreadMultiplier = 2;
                    else
                        spreadMultiplier = 1;

                    if (mask1.HasOption(Mask1.Ask))
                        bidTrade = false;
                    else
                        bidTrade = true;
                }
                if (bidTrade) {
                    barBid = barPrice;
                    barAsk = barBid + (spreadMultiplier * _defaultSpread);
                } else {
                    barAsk = barPrice;
                    barBid = barAsk - (spreadMultiplier * _defaultSpread);
                }

                //
                // check the Volume flag
                //
                int volumeMultiplier = 1;
                if (mask2.HasOption(Mask2.Volume8))
                    volumeDelta = _br.ReadUInt64BE();
                else if (mask2.HasOption(Mask2.Volume4))
                    volumeDelta = _br.ReadUInt32BE();
                else if (mask2.HasOption(Mask2.Volume2))
                    volumeDelta = _br.ReadUInt16BE();
                else if (mask2.HasOption(Mask2.Volume1X1000)) {
                    volumeDelta = _br.ReadByte();
                    volumeMultiplier = 1000;
                } else if (mask2.HasOption(Mask2.Volume1X500)) {
                    volumeDelta = _br.ReadByte();
                    volumeMultiplier = 500;
                } else if (mask2.HasOption(Mask2.Volume1))
                    volumeDelta = _br.ReadByte();
                else if (mask2.HasOption(Mask2.Volume1X100)) {
                    volumeDelta = _br.ReadByte();
                    volumeMultiplier = 100;
                } else
                    throw new Exception($"Unknown Volume Flag in Mask2: {Convert.ToString((short)mask2, 2)}\n" +
                        $"Mask1: {Convert.ToString((short)mask1, 2)}\n" +
                        $"FileName: {FileName}\n" +
                        $"Position: {_br.BaseStream.Position}");
                barVolume = (long)volumeDelta * volumeMultiplier;

                _lastDateTime = barDateTime;
                _lastPrice = barPrice;
                _lastBid = barBid;
                _lastAsk = barAsk;
                _lastVolume = barVolume;
                _lastMask1 = mask1;
                _lastMask2 = mask2;

                return new TickRecord(barDateTime, barPrice, barBid, barAsk, barVolume);
            }

        }
        public class NCDMinuteFile : NCDFile<MinuteRecord> {
            public NCDMinuteFile(string path) : base(NCDFileType.Minute, path) { }
            public override INCDRecord ReadNextRecord() {
                return ReadNextMinuteRecord();
            }

            MinuteMask1Flags _lastMask1;
            MinuteMask2Flags _lastMask2;
            private MinuteRecord ReadNextMinuteRecord() {
                MinuteMask1Flags mask1 = (MinuteMask1Flags)_br.ReadByte();
                MinuteMask2Flags mask2 = (MinuteMask2Flags)_br.ReadByte();

                // Inputs
                UInt64 timeDelta;
                uint openDelta;
                uint highDelta;
                uint lowDelta;
                uint closeDelta;
                UInt64 volumeDelta;
                uint openDeltaOffset;

                // Outputs
                DateTime barDateTime;
                Double barOpen;
                Double barHigh;
                Double barLow;
                Double barClose;
                long barVolume;

                //
                // check the Time flag
                //
                if (mask1.HasOption(MinuteMask1Flags.Time4))
                    timeDelta = _br.ReadUInt32BE();
                else if (mask1.HasOption(MinuteMask1Flags.Time1))
                    timeDelta = _br.ReadByte();
                else if (mask1.HasOption(MinuteMask1Flags.Time2))
                    timeDelta = _br.ReadUInt16BE();
                else
                    timeDelta = 1;
                barDateTime = _lastDateTime.AddMinutes(timeDelta);

                //
                // check the Open flag
                //

                if (mask1.HasOption(MinuteMask1Flags.Open4)) {
                    openDelta = _br.ReadUInt32BE();
                    openDeltaOffset = 1073741824; // 2147483648; // 1073741824; // 0x40000000
                } else if (mask1.HasOption(MinuteMask1Flags.Open1)) {
                    openDelta = _br.ReadByte();
                    openDeltaOffset = 128; // 0x80
                } else if (mask1.HasOption(MinuteMask1Flags.Open2)) {
                    openDelta = _br.ReadUInt16BE();
                    openDeltaOffset = 32768; // 0x4000
                } else {
                    openDelta = 0;
                    openDeltaOffset = 0;
                }
                barOpen = Math.Round(_lastPrice + ((openDelta - openDeltaOffset) * _tickSizePrice), 2);


                //
                // check the High flag
                //
                if (mask2.HasOption(MinuteMask2Flags.High4))
                    highDelta = _br.ReadUInt32BE();
                else if (mask2.HasOption(MinuteMask2Flags.High1))
                    highDelta = _br.ReadByte();
                else if (mask2.HasOption(MinuteMask2Flags.High2))
                    highDelta = _br.ReadUInt16BE();
                else
                    highDelta = 0;
                barHigh = Math.Round(barOpen + (highDelta * _tickSizePrice), 2);

                //
                // check the Low flag
                //
                if (mask2.HasOption(MinuteMask2Flags.Low4))
                    lowDelta = _br.ReadUInt32BE();
                else if (mask2.HasOption(MinuteMask2Flags.Low1))
                    lowDelta = _br.ReadByte();
                else if (mask2.HasOption(MinuteMask2Flags.Low2))
                    lowDelta = _br.ReadUInt16BE();
                else
                    lowDelta = 0;
                barLow = Math.Round(barOpen - (lowDelta * _tickSizePrice), 2);

                //
                // check the Close flag
                //
                if (mask2.HasOption(MinuteMask2Flags.Close4))
                    closeDelta = _br.ReadUInt32BE();
                else if (mask2.HasOption(MinuteMask2Flags.Close1))
                    closeDelta = _br.ReadByte();
                else if (mask2.HasOption(MinuteMask2Flags.Close2))
                    closeDelta = _br.ReadUInt16BE();
                else
                    closeDelta = 0;
                barClose = Math.Round(barLow + (closeDelta * _tickSizePrice), 2);

                //
                // check the Volume flag
                //
                int volumeMultiplier = 1;
                if (mask1.HasOption(MinuteMask1Flags.Volume8))
                    volumeDelta = _br.ReadUInt64BE();
                else if (mask1.HasOption(MinuteMask1Flags.Volume4))
                    volumeDelta = _br.ReadUInt32BE();
                else if (mask1.HasOption(MinuteMask1Flags.Volume2))
                    volumeDelta = _br.ReadUInt16BE();
                else if (mask1.HasOption(MinuteMask1Flags.Volume1X1000)) {
                    volumeDelta = _br.ReadByte();
                    volumeMultiplier = 1000;
                } else if (mask1.HasOption(MinuteMask1Flags.Volume1X500)) {
                    volumeDelta = _br.ReadByte();
                    volumeMultiplier = 500;
                } else if (mask1.HasOption(MinuteMask1Flags.Volume1))
                    volumeDelta = _br.ReadByte();
                else if (mask1.HasOption(MinuteMask1Flags.Volume1X100)) {
                    volumeDelta = _br.ReadByte();
                    volumeMultiplier = 100;
                } else
                    throw new Exception($"Unknown Volume Flag in Mask1: {Convert.ToString((short)mask1, 2)}\n" +
                        $"Mask2: {Convert.ToString((short)mask2, 2)}\n" +
                        $"FileName: {FileName}\n" +
                        $"Position: {_br.BaseStream.Position}");
                barVolume = (long)volumeDelta * volumeMultiplier;

                _lastDateTime = barDateTime;
                _lastPrice = barOpen;
                _lastVolume = barVolume;
                _lastMask1 = mask1;
                _lastMask2 = mask2;

                return new MinuteRecord(barDateTime, barOpen, barHigh, barLow, barClose, barVolume);
            }
        }
        public interface INCDFile {
            string FileName { get; }
            double FileStartOpen { get; }
            DateTime FileStartDateTime { get; }
            bool EndOfFile { get; }
            INCDRecord ReadNextRecord();
        }
        public abstract class NCDFile<TNCDRecord> : INCDFile
            where TNCDRecord : INCDRecord {
            public NCDFile(NCDFileType ncdFileType, string path) {
                _ncdFileType = ncdFileType;
                if (!File.Exists(path))
                    throw new FileNotFoundException();
                _br = new BinaryReader(new MemoryStream(File.ReadAllBytes(path)));

                _fileName = Path.GetFileName(path);
                _tickSizeTime = _br.ReadUInt32();
                _tickSizePrice = _br.ReadDouble();
                _fileStartPrice = _lastPrice = _br.ReadDouble();
                _fileStartDateTicks = (long)_br.ReadUInt64();
                _fileStartDateTime = _lastDateTime = TimeZoneInfo.ConvertTime(
                    new DateTime((long)_fileStartDateTicks),
                    TimeZoneInfo.Local /*ApplicationTimeZone*/);
            }

            protected NCDFileType _ncdFileType;
            protected string _fileName;
            protected UInt32 _tickSizeTime;
            protected double _tickSizePrice;
            protected double _fileStartPrice;
            protected long _fileStartDateTicks;
            protected DateTime _fileStartDateTime;
            protected DateTime _lastDateTime;
            protected double _lastPrice;
            protected double _lastVolume;

            protected private BinaryReader _br;
            public string FileName { get { return _fileName; } }
            public double FileStartOpen { get { return _fileStartPrice; } }
            public DateTime FileStartDateTime { get { return _fileStartDateTime; } }
            public bool EndOfFile { get { return _br.BaseStream.Position == _br.BaseStream.Length; } }
            public abstract INCDRecord ReadNextRecord();
        }
    }
