using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTDFileReader {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public readonly struct NCDFileTick : IEquatable<NCDFileTick> {

        public readonly DateTime Timestamp;
        public readonly double Price, Bid, Offer;
        public readonly long Volume;

        public NCDFileTick(double bid, double offer, double price, long volume, DateTime timestamp) {
            Bid = bid;
            Offer = offer;
            Price = price;
            Volume = volume;
            Timestamp = timestamp;
        }

        public override bool Equals(object obj)
            => obj is NCDFileTick tick && Equals(tick);

        public bool Equals(NCDFileTick other)
            => Bid == other.Bid
            && Offer == other.Offer
            && Price == other.Price
            && Volume == other.Volume
            && Timestamp == other.Timestamp;

        public override int GetHashCode()
            => HashCode.Combine(Timestamp, Price, Bid, Offer, Volume);
    }
}
