using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTDFileReader.Test.Properties;
using System.Text;

namespace NTDFileReader.Test {

    [TestClass]
    public class NTDFileReaderTests {

        [TestMethod]
        public void NTDFileReader_InputOutput() {

            var sb = new StringBuilder();

            foreach (var tick in NTDFileReaderUtility.Read(Resources.input)) {
                sb.AppendFormat("{0}\t{1}\t{2}\r\n", tick.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss"), tick.Price.ToString("F"), tick.Volume);
            }

            Assert.AreEqual(sb.ToString(), Resources.output);
        }
    }
}
