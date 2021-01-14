using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Nanocore.Core.Serialization
{
    internal class NativeStreamWrapper : ANativeStream
    {
        private readonly Stream _stream;
        private readonly bool _canWrite;
        private readonly long _length;

        public override bool CanRead => _stream.CanRead;
        public override bool CanWrite => _stream.CanWrite;
        public override bool CanSeek => _stream.CanSeek;
        public override bool CanTimeout => _stream.CanTimeout;
        public override long Length
        {
            get
            {
                if (!_canWrite)
                {
                    return _length;
                }
                return _stream.Length;
            }
        }
        public override long Position { get => _stream.Position; set => _stream.Position = value; }
        public override int ReadTimeout { get => _stream.ReadTimeout; set => _stream.ReadTimeout = value; }
        public override int WriteTimeout { get => _stream.WriteTimeout; set => _stream.WriteTimeout = value; }

        public NativeStreamWrapper(Stream stream)
        {
            _stream = stream;
            _canWrite = stream.CanWrite;
            _length = stream.Length;
        }

        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        public override int ReadByte()
        {
            return _stream.ReadByte();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        public override void WriteByte(byte value)
        {
            _stream.WriteByte(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return _stream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return _stream.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            return _stream.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return _stream.FlushAsync(cancellationToken);
        }
    }
}
