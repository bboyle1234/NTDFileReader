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
    public class NCDTicksTests {

        [TestMethod]
        public void NCDTicks_01() {
            using var stream = new MemoryStream(Resources.NQTicksInput);
            using var enumeratorIn = NCDFileReaderUtility.Read(stream).GetEnumerator();
            using var enumeratorOut = Resources.NQTicksOutput.Split('\n').Select(line => {
                var parts = line.Trim().Split(';');
                var timestamp = DateTime.ParseExact(parts[0], "yyyyMMdd HHmmss fffffff", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                var price = double.Parse(parts[1], CultureInfo.InvariantCulture);
                var bid = double.Parse(parts[2], CultureInfo.InvariantCulture);
                var ask = double.Parse(parts[3], CultureInfo.InvariantCulture);
                var volume = long.Parse(parts[4], CultureInfo.InvariantCulture);
                return new NCDFileTick(bid, ask, price, volume, timestamp);
            }).GetEnumerator();
            while (enumeratorIn.MoveNext()) {
                if (!enumeratorOut.MoveNext()) throw new Exception();
                if (!enumeratorIn.Current.Equals(enumeratorOut.Current))
                    throw new Exception("");
            }
            if (enumeratorOut.MoveNext()) throw new Exception();
        }
    }
}
