using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NTDFileReader {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public readonly struct NTDFileTick : IEquatable<NTDFileTick> {

        public DateTime TimeStamp { get; }

        public decimal Price { get; }

        public ulong Volume { get;  }

        public NTDFileTick(DateTime timestamp, decimal price, ulong volume) {
            TimeStamp = timestamp;
            Price = price;
            Volume = volume;
        }

        public override bool Equals(object obj)
            => obj is NTDFileTick tick && Equals(tick);
       
        public bool Equals(NTDFileTick other) 
            => TimeStamp == other.TimeStamp 
            && Price == other.Price 
            && Volume == other.Volume;

        public override int GetHashCode() 
            => HashCode.Combine(TimeStamp, Price, Volume);
    }
}
