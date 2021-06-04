using Nanocore;
using Nanocore.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NanoswarmHive
{
    internal class ExecutableCharateristics
    {
        private readonly uint[] _hashes;
        public ExecutableType Type { get; }

        public static List<ExecutableCharateristics> GetSupportedVersions()
        {
            return new List<ExecutableCharateristics>();
        }

        public static List<ExecutableCharateristics> GetSupportedVersionsFromFile(string path)
        {
            var list = new List<ExecutableCharateristics>();
            var separators = new[] { ' ' };
            return File.ReadAllLines(path).AsParallel().Select(line =>
            {
                var splitted = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                if (splitted.Length < 2 || !Enum.TryParse<ExecutableType>(splitted[0], out var type))
                {
                    return null;
                }
                var hashes = splitted.Skip(1).Select(uint.Parse);
                return new ExecutableCharateristics(type, hashes);
            }).Where(x => x != null).ToList();
        }

        public ExecutableCharateristics(ExecutableType type, IEnumerable<uint> hashes)
        {
            _hashes = hashes.ToArray();
            Type = type;
        }

        public double GetSimilarity(IReadOnlyList<uint> hashes)
        {
            var minSize = Math.Min(_hashes.Length, hashes.Count);
            var maxSize = Math.Max(_hashes.Length, hashes.Count);
            var delta = 1.0 / maxSize;
            var similarity = 0.0;
            for (var i = 0; i < minSize; ++i)
            {
                if (_hashes[i] == hashes[i])
                {
                    similarity += delta;
                }
            }
            return similarity;
        }
    }

    internal static class ExecutableCharateristicsUtilities
    {
        public static List<uint> ReadHashes(this Stream stream)
        {
            var reader = new BinaryReader(stream, System.Text.Encoding.ASCII, true);
            var hashes = new List<uint>();
            while (true)
            {
                var bytesRead = reader.ReadBytes(4096);
                if (bytesRead.Length > 0)
                {
                    hashes.Add(FastHash.GetHashCode(bytesRead, bytesRead.Length));
                }
                if (bytesRead.Length < 4096)
                {
                    break;
                }
            }
            return hashes;
        }

        public static ExecutableType DetectExecutableType(this IList<ExecutableCharateristics> supportedVersions,
                                                          List<uint> hashes,
                                                          double threshold,
                                                          out double similarity)
        {
            var result = supportedVersions.AsParallel()
                .Select(e => (Candidate: e, Similarity: e.GetSimilarity(hashes)))
                .Where(t => t.Similarity >= threshold)
                .OrderByDescending(t => t.Similarity)
                .FirstOrDefault();
            if (result == default)
            {
                similarity = 0;
                return ExecutableType.Unknown;
            }

            similarity = result.Similarity;
            return result.Candidate.Type;
        }
    }
}
