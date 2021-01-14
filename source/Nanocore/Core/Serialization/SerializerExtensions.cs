using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Nanocore.Core.Serialization
{
    public static class SerializerExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadBool(this ASerializationStream stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            bool result = false;
            stream.Serialize(ref result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadBool32(this ASerializationStream stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            uint result = 0;
            stream.Serialize(ref result);
            return result != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadByte(this ASerializationStream stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            byte result = 0;
            stream.Serialize(ref result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadSByte(this ASerializationStream stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            sbyte result = 0;
            stream.Serialize(ref result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUInt16(this ASerializationStream stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            ushort result = 0;
            stream.Serialize(ref result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadInt16(this ASerializationStream stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            short result = 0;
            stream.Serialize(ref result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt32(this ASerializationStream stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            uint result = 0;
            stream.Serialize(ref result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt32(this ASerializationStream stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            int result = 0;
            stream.Serialize(ref result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadUInt64(this ASerializationStream stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            ulong result = 0;
            stream.Serialize(ref result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadInt64(this ASerializationStream stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            long result = 0;
            stream.Serialize(ref result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadFloat(this ASerializationStream stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            float result = 0;
            stream.Serialize(ref result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReadDouble(this ASerializationStream stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            double result = 0;
            stream.Serialize(ref result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadChar(this ASerializationStream stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            char result = '\0';
            stream.Serialize(ref result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadString(this ASerializationStream stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            string result = null;
            stream.Serialize(ref result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadString(this ASerializationStream stream, int length)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            string result = null;
            stream.Serialize(ref result, length);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe string ReadWideString(this ASerializationStream stream, int length)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            byte[] buffer = new byte[length << 1];
            stream.Serialize(buffer, 0, buffer.Length);
            fixed (byte* pChars = buffer)
            {
                return new string((sbyte*)pChars, 0, buffer.Length, Encoding.Unicode);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe string ReadWideStringNegate(this ASerializationStream stream, int length)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            byte[] buffer = new byte[length << 1];
            stream.Serialize(buffer, 0, buffer.Length);
            for (int idx = 0; idx < buffer.Length; ++idx)
            {
                buffer[idx] = (byte)~buffer[idx];
            }
            fixed (byte* pChars = buffer)
            {
                return new string((sbyte*)pChars, 0, buffer.Length, Encoding.Unicode);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe string ReadUnicodeStringNegate(this ASerializationStream stream, int length)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            string result = null;
            stream.SerializeUnicodeNegate(ref result, length);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ASerializationStream Write(this ASerializationStream stream, string value)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            stream.Serialize(ref value);
            return stream;
        }
    }
}
