using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTDFileReader {

    internal static class BinaryReaderExtensions {

        public static int ReadBigEndianInt(this BinaryReader br, int byteCount) {
            var result = (int)br.ReadByte();
            for(var i = 1; i < byteCount; i++) {
                result <<= 8;
                result += br.ReadByte();
            }
            return result;
        }

        public static long ReadBigEndianLong(this BinaryReader br, int byteCount) {
            var result = (long)br.ReadByte();
            for (var i = 1; i < byteCount; i++) {
                result <<= 8;
                result += br.ReadByte();
            }
            return result;
        }

        public static uint ReadBigEndianUInt(this BinaryReader br, int byteCount) {
            var result = (uint)br.ReadByte();
            for (var i = 1; i < byteCount; i++) {
                result <<= 8;
                result += br.ReadByte();
            }
            return result;
        }

        public static ulong ReadBigEndianULong(this BinaryReader br, int byteCount) {
            var result = (ulong)br.ReadByte();
            for (var i = 1; i < byteCount; i++) {
                result <<= 8;
                result += br.ReadByte();
            }
            return result;
        }
    }
}
