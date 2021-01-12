using Nanocore.Native;
using System;
using System.Runtime.InteropServices;

namespace Nanocore.Sage
{
    public static class RegistryClass
    {
        public static readonly HKey HKEY_CURRENT_USER = new HKey(0x80000001u);
        public static readonly HKey HKEY_LOCAL_MACHINE = new HKey(0x80000002u);

        private static bool QueryValueLengthA(HKey hKey, string lpValueName, ref uint lpcbData)
        {
            AdvApi32.RegistryValueType type = AdvApi32.RegistryValueType.Binary;
            return AdvApi32.RegQueryValueExA(hKey, lpValueName, 0, ref type, IntPtr.Zero, ref lpcbData) == 0;
        }

        private static bool QueryValueLengthW(HKey hKey, string lpValueName, ref uint lpcbData)
        {
            AdvApi32.RegistryValueType type = AdvApi32.RegistryValueType.Binary;
            return AdvApi32.RegQueryValueExW(hKey, lpValueName, 0, ref type, IntPtr.Zero, ref lpcbData) == 0;
        }

        private static bool QueryValueStringA(HKey hKey, string lpValueName, IntPtr lpData, uint cbData)
        {
            AdvApi32.RegistryValueType type = AdvApi32.RegistryValueType.Sz;
            return AdvApi32.RegQueryValueExA(hKey, lpValueName, 0, ref type, lpData, ref cbData) == 0;
        }

        private static bool QueryValueStringW(HKey hKey, string lpValueName, IntPtr lpData, uint cbData)
        {
            AdvApi32.RegistryValueType type = AdvApi32.RegistryValueType.Sz;
            return AdvApi32.RegQueryValueExW(hKey, lpValueName, 0, ref type, lpData, ref cbData) == 0;
        }

        public static bool OpenKey(HKey key, string subKey, out HKey result)
        {
            return AdvApi32.RegOpenKeyExW(key, subKey, 0, AdvApi32.RegistryKeyAccess.Read, out result) == 0;
        }

        public static bool GetDWord(HKey key, string value, out int result)
        {
            uint cbData = 4;
            AdvApi32.RegistryValueType type = AdvApi32.RegistryValueType.DWord;
            IntPtr resultValue = MemoryPool.Allocate(4);
            bool ret = false;
            if (AdvApi32.RegQueryValueExW(key, value, 0, ref type, resultValue, ref cbData) == 0)
            {
                result = Marshal.PtrToStructure<int>(resultValue);
                ret = true;
            }
            else
            {
                result = 0;
            }
            MemoryPool.Free(resultValue);
            return ret;
        }

        public static bool GetStringA(HKey key, string value, out string result)
        {
            uint cbData = 0;
            IntPtr resultData;
            bool ret = false;
            if (QueryValueLengthA(key, value, ref cbData))
            {
                resultData = MemoryPool.Allocate((int)(cbData + 1));
                if (QueryValueStringA(key, value, resultData, cbData + 1))
                {
                    result = Marshal.PtrToStringAnsi(resultData);
                    ret = true;
                }
                else
                {
                    result = null;
                }
            }
            else
            {
                result = null;
            }
            return ret;
        }

        public static bool GetStringW(HKey key, string value, out string result)
        {
            uint cbData = 0;
            IntPtr resultData;
            bool ret = false;
            if (QueryValueLengthW(key, value, ref cbData))
            {
                resultData = MemoryPool.Allocate((int)((2 * (cbData + 1)) | -((long)(cbData + 1) >> 31)));
                if (QueryValueStringW(key, value, resultData, cbData + 1))
                {
                    result = Marshal.PtrToStringUni(resultData);
                    ret = true;
                }
                else
                {
                    result = null;
                }
            }
            else
            {
                result = null;
            }
            return ret;
        }
    }
}
