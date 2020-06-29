using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTDFileReader.Test.Properties;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Globalization;

namespace NTDFileReader.Test {

    [TestClass]
    public class NCD {
    
        [TestMethod]
        public void Ticks() {
            using var stream = new MemoryStream(Resources.NQ202006231900_Input);
            using var enumeratorIn = NCDUtility.ReadTicks(stream).GetEnumerator();
            using var enumeratorOut = Resources.NQ202006231900_Output.Split('\n').Select(line => {
                var parts = line.Trim().Split('\t');
                var timestamp = DateTime.ParseExact(parts[0], "yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                var price = double.Parse(parts[4], CultureInfo.InvariantCulture);
                var bid = double.Parse(parts[5], CultureInfo.InvariantCulture);
                var ask = double.Parse(parts[6], CultureInfo.InvariantCulture);
                var volume = ulong.Parse(parts[7], CultureInfo.InvariantCulture);
                return new NCDTick(bid, ask, price, volume, timestamp);
            }).GetEnumerator();
            while (enumeratorIn.MoveNext()) {
                if (!enumeratorOut.MoveNext()) throw new Exception("Uneven number of ticks");
                if (!enumeratorIn.Current.Equals(enumeratorOut.Current))
                    throw new Exception("Ticks do not match");
            }
            if (enumeratorOut.MoveNext()) throw new Exception("Uneven number of ticks");
        }

        [TestMethod]
        public void Minutes() {
            using var stream = new MemoryStream(Resources.NQMinutes_Last);
            using var enumeratorIn = NCDUtility.ReadMinutes(stream).GetEnumerator();
            using var enumeratorOut = Resources.NQMinutes.Split('\n').Select(line => {
                var parts = line.Trim().Split('\t');
                var timestamp = DateTime.ParseExact(parts[0], "yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                var open = double.Parse(parts[1], CultureInfo.InvariantCulture);
                var high = double.Parse(parts[2], CultureInfo.InvariantCulture);
                var low = double.Parse(parts[3], CultureInfo.InvariantCulture);
                var close = double.Parse(parts[4], CultureInfo.InvariantCulture);
                var volume = ulong.Parse(parts[5], CultureInfo.InvariantCulture);
                return new NCDMinute(open, high, low, close, volume, timestamp);
            }).GetEnumerator();
            while (enumeratorIn.MoveNext()) {
                if (!enumeratorOut.MoveNext()) throw new Exception("Uneven number of ticks");
                if (!enumeratorIn.Current.Equals(enumeratorOut.Current))
                    throw new Exception("Ticks do not match");
            }
            if (enumeratorOut.MoveNext()) throw new Exception("Uneven number of ticks");
        }
    }
}
