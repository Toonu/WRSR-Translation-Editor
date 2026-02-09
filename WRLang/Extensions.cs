using System.Buffers.Binary;

namespace WRLang {
    internal static class Extensions {
        public static int ReadInt32BigEndian(this BinaryReader reader) {
            return BinaryPrimitives.ReadInt32BigEndian(reader.ReadBytes(4));
        }

        public static short ReadInt16BigEndian(this BinaryReader reader) {
            return BinaryPrimitives.ReadInt16BigEndian(reader.ReadBytes(2));
        }

        public static void WriteInt32BigEndian(this BinaryWriter writer, int value) {
            Span<byte> bytes = stackalloc byte[4];
            BinaryPrimitives.WriteInt32BigEndian(bytes, value);
            writer.Write(bytes);
        }

        public static void WriteInt16BigEndian(this BinaryWriter writer, short value) {
            Span<byte> bytes = stackalloc byte[2];
            BinaryPrimitives.WriteInt16BigEndian(bytes, value);
            writer.Write(bytes);
        }
    }
}
