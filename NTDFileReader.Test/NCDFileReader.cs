using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTDFileReader.Test.Properties;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.IO;

namespace NTDFileReader.Test {

    [TestClass]
    public class NCDFileReaderTests {

        [TestMethod]
        public void NCDFileReader_1() {

            var inputBytes = Resources.ncdInput;
            //var hex = BitConverter.ToString(inputBytes).Replace("-", " ");
            //File.WriteAllText("D:/hex.txt", hex);
            //Process.Start("D:/hex.txt");




            var expectedNCDFileBars = Resources.ncdOutput
                .Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Split('\t'))
                .Select(parts => new NCDFileBar {
                    Timestamp = DateTime.ParseExact(parts[0], "yyyy-MM-dd HH:mm:ss.fff", null),
                    Open = double.Parse(parts[1]),
                    High = double.Parse(parts[2]),
                    Low = double.Parse(parts[3]),
                    Close = double.Parse(parts[4]),
                    Volume = long.Parse(parts[5]),
                }).ToArray();

            var searchValue = -expectedNCDFileBars[0].Open;

            //Array.Reverse(inputBytes);
            for (var i = 0; i < inputBytes.Length - 8; i++) {
                var foundValue = BitConverter.ToDouble(inputBytes, i);
                if (foundValue == searchValue) {
                    Debugger.Break();
                }
            }

            for (var i = 0; i < inputBytes.Length - 8; i++) {
                using (var ms = new MemoryStream(inputBytes)) {
                    using (var br = new BinaryReader(ms)) {
                        for (var j = 0; j < i; j++) br.ReadByte();
                        var foundValue = br.ReadDouble();
                        if (foundValue == searchValue) {
                            Debugger.Break();
                        }
                    }
                }
            }

            for (var i = 0; i < inputBytes.Length - 4; i++) {
                var foundValue = BitConverter.ToSingle(inputBytes, i);
                if (foundValue == searchValue) {
                    Debugger.Break();
                }
            }

            for (var i = 0; i < inputBytes.Length - 4; i++) {
                using (var ms = new MemoryStream(inputBytes)) {
                    using (var br = new BinaryReader(ms)) {
                        for (var j = 0; j < i; j++) br.ReadByte();
                        var foundValue = br.ReadSingle();
                        if (foundValue == searchValue) {
                            Debugger.Break();
                        }
                    }
                }
            }
        }
    }
}
