using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTDFileReader.Test.Properties;

namespace NTDFileReader.Test {

    [TestClass]
    public class NTD {

        [TestMethod]
        public void Ticks() {

            using var inputStream = new MemoryStream(Resources.ntdInput);
            using var inputEnumerator = NTDUtility.ReadTicks(inputStream).GetEnumerator();

            using var outputEnumerator = Resources.ntdOutput.Split('\n').Select(line => {
                var parts = line.Trim().Split('\t');
                var time = DateTime.ParseExact(parts[0], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                var price = double.Parse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture);
                var volume = ulong.Parse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture);
                return new NTDTick(time, price, volume);
            }).GetEnumerator();


            while (inputEnumerator.MoveNext()) {
                if (!outputEnumerator.MoveNext()) throw new Exception("Uneven number of ticks");
                if (!inputEnumerator.Current.Equals(outputEnumerator.Current))
                    throw new Exception("Ticks do not match");
            }
            if (outputEnumerator.MoveNext()) throw new Exception("Uneven number of ticks");
        }
    }
}
