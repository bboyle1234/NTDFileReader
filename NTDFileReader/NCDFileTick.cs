using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTDFileReader {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    
    public readonly struct NCDFileTick : IEquatable<NCDFileTick> {

        public readonly DateTime Timestamp;
        public readonly double Price, Bid, Ask;
        public readonly long Volume;

        public NCDFileTick(double bid, double ask, double price, long volume, DateTime timestamp) {
            Bid = bid;
            Ask = ask;
            Price = price;
            Volume = volume;
            Timestamp = timestamp;
        }

        public override bool Equals(object obj) 
            => Equals((NCDFileTick)obj);

        public bool Equals(NCDFileTick other)
            => Bid == other.Bid && Ask == other.Ask && Price == other.Price && Volume == other.Volume && Timestamp == other.Timestamp;

    }
}
