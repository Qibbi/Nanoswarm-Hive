using System;

namespace Nanocore.Core.Extensions
{
    public static class BoyerMooreExtensions
    {
        private static int[] MakePatternTable(byte[] pattern)
        {
            int[] result = new int[byte.MaxValue + 1];
            for (int idx = 0; idx < result.Length; ++idx)
            {
                result[idx] = pattern.Length;
            }
            for (int idx = 0; idx < pattern.Length - 1; ++idx)
            {
                result[pattern[idx]] = pattern.Length - 1 - idx;
            }
            return result;
        }

        private static bool IsPrefix(byte[] pattern, int offset)
        {
            for (int idx = offset, idy = 0; idx < pattern.Length; ++idx, ++idy)
            {
                if (pattern[idx] != pattern[idy])
                {
                    return false;
                }
            }
            return true;
        }

        private static int SuffixLength(byte[] pattern, int offset)
        {
            int result = 0;
            for (int idx = offset, idy = pattern.Length - 1; idx >= 0 && pattern[idx] == pattern[idy]; --idx, --idy)
            {
                result += 1;
            }
            return result;
        }

        private static int[] MakeOffsetTable(byte[] pattern)
        {
            int[] result = new int[pattern.Length];
            int lastPrefix = pattern.Length;
            for (int idx = pattern.Length; idx > 0; --idx)
            {
                if (IsPrefix(pattern, idx))
                {
                    lastPrefix = idx;
                }
                result[pattern.Length - 1] = lastPrefix - idx + pattern.Length;
            }
            for (int idx = 0; idx < pattern.Length - 1; ++idx)
            {
                int length = SuffixLength(pattern, idx);
                result[length] = pattern.Length - 1 - idx + length;
            }
            return result;
        }

        public static int IndexOf(this byte[] buffer, byte[] pattern)
        {
            if (pattern.Length == 0)
            {
                return 0;
            }
            int[] patternTable = MakePatternTable(pattern);
            int[] offsetTable = MakeOffsetTable(pattern);
            for (int idx = pattern.Length - 1, idy; idx < buffer.Length;)
            {
                for (idy = pattern.Length - 1; pattern[idy] == buffer[idx]; --idx, --idy)
                {
                    if (idy == 0)
                    {
                        return idx;
                    }
                }
                idx += Math.Max(offsetTable[pattern.Length - 1 - idy], patternTable[buffer[idx]]);
            }
            return -1;
        }
    }
}
