using System;
using System.IO;

namespace Nanocore.Core.Serialization
{
    public abstract class ASerializationStream
    {
        private sealed class InternalScopedSeek : IDisposable
        {
            private readonly ASerializationStream _stream;
            private readonly long _position;

            public InternalScopedSeek(ASerializationStream stream, long offset)
            {
                _stream = stream;
                _position = stream.NativeStream.Position;
                stream.NativeStream.Seek(offset, SeekOrigin.Begin);
            }

            public void Dispose()
            {
                _stream.NativeStream.Seek(_position, SeekOrigin.Begin);
            }
        }

        protected const int BufferSize = 1024;

        [ThreadStatic] protected static byte[] Buffer;

        public ANativeStream NativeStream { get; protected set; }

        public abstract void Serialize(ref bool value);

        public abstract void Serialize(ref byte value);

        public abstract void Serialize(ref sbyte value);

        public abstract void Serialize(ref ushort value);

        public abstract void Serialize(ref short value);

        public abstract void Serialize(ref uint value);

        public abstract void Serialize(ref int value);

        public abstract void Serialize(ref ulong value);

        public abstract void Serialize(ref long value);

        public abstract void Serialize(ref float value);

        public abstract void Serialize(ref double value);

        public abstract void Serialize(ref char value);

        public abstract void SerializeUnicode(ref char value);

        public abstract void Serialize(ref string value);

        public abstract void SerializeUnicode(ref string value);

        public abstract void Serialize(ref string value, int length);

        public abstract void SerializeUnicode(ref string value, int length);

        public abstract void SerializeUnicodeNegate(ref string value, int length);

        public abstract void Serialize(byte[] values, int offset, int count);

        public abstract void Serialize(IntPtr ptr, int count);

        public abstract void Flush();

        public IDisposable GoTo(long offset)
        {
            return new InternalScopedSeek(this, offset);
        }
    }
}
