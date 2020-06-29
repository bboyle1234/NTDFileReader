using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTDFileReader {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    /// Represents a minute from a NinjaTrader 8 .ncd file.
    /// </summary>
    public readonly struct NCDMinute : IEquatable<NCDMinute> {

        public readonly DateTime Timestamp;
        public readonly double Open, High, Low, Close;
        public readonly ulong Volume;

        public NCDMinute(double open, double high, double low, double close, ulong volume, DateTime timestamp) {
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
            Timestamp = timestamp;
        }

        public override bool Equals(object obj)
            => obj is NCDMinute tick && Equals(tick);

        public bool Equals(NCDMinute other)
            => Open == other.Open
            && High == other.High
            && Low == other.Low
            && Close == other.Close
            && Volume == other.Volume
            && Timestamp == other.Timestamp;

        public override int GetHashCode()
            => HashCode.Combine(Timestamp, Open, High, Low, Close, Volume);

        public static bool operator ==(NCDMinute left, NCDMinute right)
            => left.Equals(right);

        public static bool operator !=(NCDMinute left, NCDMinute right)
            => !left.Equals(right);
    }
}
