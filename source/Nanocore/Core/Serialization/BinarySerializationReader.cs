using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Nanocore.Core.Serialization
{
    public sealed class BinarySerializationReader : ASerializationStream
    {
        private static readonly List<byte> _charBuffer = new List<byte>();
        private static readonly List<ushort> _wideCharBuffer = new List<ushort>();

        private static byte[] _tempBuffer;

        private readonly BinaryReader _reader;

        public BinarySerializationReader(Stream stream) : base()
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            _reader = new BinaryReader(stream);
            NativeStream = stream.ToNativeStream();
        }

        public override void Serialize(ref bool value)
        {
            int result = NativeStream.ReadByte();
            if (result == -1) throw new EndOfStreamException();
            value = result != 0;
        }

        public override void Serialize(ref byte value)
        {
            int result = NativeStream.ReadByte();
            if (result == -1) throw new EndOfStreamException();
            value = (byte)result;
        }

        public override void Serialize(ref sbyte value)
        {
            int result = NativeStream.ReadByte();
            if (result == -1) throw new EndOfStreamException();
            value = (sbyte)(byte)result;
        }

        public override void Serialize(ref ushort value)
        {
            value = NativeStream.ReadUInt16();
        }

        public override void Serialize(ref short value)
        {
            value = (short)NativeStream.ReadUInt16();
        }

        public override void Serialize(ref uint value)
        {
            value = NativeStream.ReadUInt32();
        }

        public override void Serialize(ref int value)
        {
            value = (int)NativeStream.ReadUInt32();
        }

        public override void Serialize(ref ulong value)
        {
            value = NativeStream.ReadUInt64();
        }

        public override void Serialize(ref long value)
        {
            value = (long)NativeStream.ReadUInt64();
        }

        public override unsafe void Serialize(ref float value)
        {
            fixed (float* pValue = &value)
            {
                *(uint*)pValue = NativeStream.ReadUInt32();
            }
        }

        public override unsafe void Serialize(ref double value)
        {
            fixed (double* pValue = &value)
            {
                *(ulong*)pValue = NativeStream.ReadUInt64();
            }
        }

        public override void Serialize(ref char value)
        {
            byte c = 0;
            Serialize(ref c);
            value = (char)c;
        }

        public override void SerializeUnicode(ref char value)
        {
            value = (char)NativeStream.ReadUInt16();
        }

        public override unsafe void Serialize(ref string value)
        {
            _charBuffer.Clear();
            byte c = 0;
            while (NativeStream.Position < NativeStream.Length)
            {
                Serialize(ref c);
                if (c == 0)
                {
                    break;
                }
                _charBuffer.Add(c);
            }
            byte[] buffer = _charBuffer.ToArray();
            fixed (byte* pChars = buffer)
            {
                value = new string((sbyte*)pChars, 0, buffer.Length, Encoding.UTF8);
            }
        }

        public override unsafe void SerializeUnicode(ref string value)
        {
            _wideCharBuffer.Clear();
            ushort c = 0;
            while (NativeStream.Position < NativeStream.Length)
            {
                Serialize(ref c);
                if (c == 0)
                {
                    break;
                }
                _wideCharBuffer.Add(c);
            }
            ushort[] buffer = _wideCharBuffer.ToArray();
            fixed (ushort* pChars = buffer)
            {
                value = new string((sbyte*)pChars, 0, buffer.Length << 1, Encoding.Unicode);
            }
        }

        public override unsafe void Serialize(ref string value, int length)
        {
            if (_tempBuffer is null || _tempBuffer.Length < length)
            {
                _tempBuffer = new byte[length];
            }
            Serialize(_tempBuffer, 0, length);
            fixed (byte* pChars = _tempBuffer)
            {
                value = new string((sbyte*)pChars, 0, length, Encoding.UTF8);
            }
        }

        public override unsafe void SerializeUnicode(ref string value, int length)
        {
            length <<= 1;
            if (_tempBuffer is null || _tempBuffer.Length < length)
            {
                _tempBuffer = new byte[length];
            }
            Serialize(_tempBuffer, 0, length);
            fixed (byte* pChars = _tempBuffer)
            {
                value = new string((sbyte*)pChars, 0, length, Encoding.Unicode);
            }
        }

        public override unsafe void SerializeUnicodeNegate(ref string value, int length)
        {
            length <<= 1;
            if (_tempBuffer is null || _tempBuffer.Length < length)
            {
                _tempBuffer = new byte[length];
            }
            Serialize(_tempBuffer, 0, length);
            fixed (byte* pChars = _tempBuffer)
            {
                byte* pC = pChars;
                for (int idx = 0; idx < length; ++idx)
                {
                    *pC = (byte)~*pC;
                    ++pC;
                }
                value = new string((sbyte*)pChars, 0, length, Encoding.Unicode);
            }
        }

        public override void Serialize(byte[] values, int offset, int count)
        {
            _reader.Read(values, offset, count);
        }

        public override void Serialize(IntPtr ptr, int count)
        {
            NativeStream.Read(ptr, count);
        }

        public override void Flush()
        {
        }
    }
}
