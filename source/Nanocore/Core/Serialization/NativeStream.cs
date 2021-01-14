using System;
using System.IO;

namespace Nanocore.Core.Serialization
{
    public abstract class ANativeStream : Stream
    {
        protected const int BufferSize = 1024;

        protected byte[] Buffer;

        public virtual unsafe ushort ReadUInt16()
        {
            byte[] buffer = Buffer;
            if (buffer is null)
            {
                buffer = Buffer = new byte[BufferSize];
            }
            int bytesRead = Read(buffer, 0, sizeof(ushort));
            if (bytesRead != sizeof(ushort)) throw new InvalidOperationException("End of stream.");
            fixed (byte* pBuffer = buffer)
            {
                return *(ushort*)pBuffer;
            }
        }

        public virtual unsafe uint ReadUInt32()
        {
            byte[] buffer = Buffer;
            if (buffer is null)
            {
                buffer = Buffer = new byte[BufferSize];
            }
            int bytesRead = Read(buffer, 0, sizeof(uint));
            if (bytesRead != sizeof(uint)) throw new InvalidOperationException("End of stream.");
            fixed (byte* pBuffer = buffer)
            {
                return *(uint*)pBuffer;
            }
        }

        public virtual unsafe ulong ReadUInt64()
        {
            byte[] buffer = Buffer;
            if (buffer is null)
            {
                buffer = Buffer = new byte[BufferSize];
            }
            int bytesRead = Read(buffer, 0, sizeof(ulong));
            if (bytesRead != sizeof(ulong)) throw new InvalidOperationException("End of stream.");
            fixed (byte* pBuffer = buffer)
            {
                return *(ulong*)pBuffer;
            }
        }

        public virtual unsafe int Read(IntPtr ptr, int count)
        {
            byte[] buffer = Buffer;
            if (buffer is null)
            {
                buffer = Buffer = new byte[BufferSize];
            }
            int bytesRead = 0;
            for (int idx = 0; idx < count; idx += BufferSize, ptr += BufferSize)
            {
                int block = count - idx;
                if (block > BufferSize)
                {
                    block = BufferSize;
                }
                int blockRead = Read(buffer, 0, block);
                bytesRead += blockRead;
                fixed (byte* pBuffer = buffer)
                {
                    Utils.CopyMemory(ptr, (IntPtr)pBuffer, block);
                }
                if (blockRead < block)
                {
                    break;
                }
            }
            return bytesRead;
        }

        public virtual unsafe void Write(ushort value)
        {
            byte[] buffer = Buffer;
            if (buffer is null)
            {
                buffer = Buffer = new byte[BufferSize];
            }
            fixed (byte* pBuffer = buffer)
            {
                *(ushort*)pBuffer = value;
            }
            Write(buffer, 0, sizeof(ushort));
        }

        public virtual unsafe void Write(uint value)
        {
            byte[] buffer = Buffer;
            if (buffer is null)
            {
                buffer = Buffer = new byte[BufferSize];
            }
            fixed (byte* pBuffer = buffer)
            {
                *(uint*)pBuffer = value;
            }
            Write(buffer, 0, sizeof(uint));
        }

        public virtual unsafe void Write(ulong value)
        {
            byte[] buffer = Buffer;
            if (buffer is null)
            {
                buffer = Buffer = new byte[BufferSize];
            }
            fixed (byte* pBuffer = buffer)
            {
                *(ulong*)pBuffer = value;
            }
            Write(buffer, 0, sizeof(ulong));
        }

        public virtual unsafe void Write(IntPtr ptr, int count)
        {
            byte[] buffer = Buffer;
            if (buffer is null)
            {
                buffer = Buffer = new byte[BufferSize];
            }
            for (int idx = 0; idx < count; idx += BufferSize, ptr += BufferSize)
            {
                int block = count - idx;
                if (block > BufferSize)
                {
                    block = BufferSize;
                }
                fixed (byte* pBuffer = buffer)
                {
                    Utils.CopyMemory((IntPtr)pBuffer, ptr, block);
                }
                Write(buffer, 0, block);
            }
        }
    }
}
