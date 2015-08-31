using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NTDFileReader {
    [DataContract]
    public class NTDFileTick {

        [DataMember(Name = "t")]
        public DateTime TimeStamp { get; set; }

        [DataMember(Name = "p")]
        public decimal Price { get; set; }

        [DataMember(Name = "v")]
        public ulong Volume { get; set; }
    }
}
