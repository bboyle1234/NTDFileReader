using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTDFileReader {
    public class NCDFileTick {
        public DateTime Timestamp;
        public double Price, Bid, Ask;
        public long Volume;
    }
}
