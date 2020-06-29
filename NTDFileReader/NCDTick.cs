using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTDFileReader {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    /// Represents a tick from a NinjaTrader 8 .ncd file.
    /// </summary>
    public readonly struct NCDTick : IEquatable<NCDTick> {

        public readonly DateTime Timestamp;
        public readonly double Price, Bid, Offer;
        public readonly ulong Volume;

        public NCDTick(double bid, double offer, double price, ulong volume, DateTime timestamp) {
            Bid = bid;
            Offer = offer;
            Price = price;
            Volume = volume;
            Timestamp = timestamp;
        }

        public override bool Equals(object obj)
            => obj is NCDTick tick && Equals(tick);

        public bool Equals(NCDTick other)
            => Bid == other.Bid
            && Offer == other.Offer
            && Price == other.Price
            && Volume == other.Volume
            && Timestamp == other.Timestamp;

        public override int GetHashCode()
            => HashCode.Combine(Timestamp, Price, Bid, Offer, Volume);

        public static bool operator ==(NCDTick left, NCDTick right)
            => left.Equals(right);

        public static bool operator !=(NCDTick left, NCDTick right)
            => !left.Equals(right);
    }
}
