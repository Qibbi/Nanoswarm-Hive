using System.IO;

namespace Nanocore.Core.Serialization
{
    public static class NativeStreamExtensions
    {
        public static ANativeStream ToNativeStream(this Stream stream)
        {
            if (stream is ANativeStream nativeStream)
            {
                return nativeStream;
            }
            return new NativeStreamWrapper(stream);
        }
    }
}
