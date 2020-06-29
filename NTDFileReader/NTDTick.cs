using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NTDFileReader {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    /// Represents a tick read from a NinjaTrader 7 .ntd file.
    /// </summary>
    public readonly struct NTDTick : IEquatable<NTDTick> {

        public readonly DateTime TimeStamp;
        public readonly double Price;
        public readonly ulong Volume;

        public NTDTick(DateTime timestamp, double price, ulong volume) {
            TimeStamp = timestamp;
            Price = price;
            Volume = volume;
        }

        public override bool Equals(object obj)
            => obj is NTDTick tick && Equals(tick);

        public bool Equals(NTDTick other)
            => TimeStamp == other.TimeStamp
            && Price == other.Price
            && Volume == other.Volume;

        public override int GetHashCode()
            => HashCode.Combine(TimeStamp, Price, Volume);

        public static bool operator ==(NTDTick left, NTDTick right) 
            => left.Equals(right);

        public static bool operator !=(NTDTick left, NTDTick right)
            => !left.Equals(right);
    }
}
