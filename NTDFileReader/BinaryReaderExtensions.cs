using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTDFileReader {

    public static class BinaryReaderExtensions {

        public static int GetBigEndianInt(this BinaryReader br, int byteCount) {
            var result = 0;
            for (int i = 0; i < byteCount; i++) {
                result = result << 8;
                result += br.ReadByte();
            }
            return result;
        }

        public static uint ReadBigEndianUInt(this BinaryReader br, int byteCount) {
            var result = 0u;
            for (var i = 0; i < byteCount; i++) {
                result = result << 8;
                result += br.ReadByte();
            }
            return result;
        }

        public static uint ReadLittleEndianUInt(this BinaryReader br, int byteCount) {
            var result = 0u;
            for (var i = 0; i < byteCount; i++) {
                result += ((uint)br.ReadByte()) << (i * 8);
            }
            return result;
        }
    }
}
