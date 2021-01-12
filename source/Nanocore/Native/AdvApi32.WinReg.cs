using System;
using System.Runtime.InteropServices;

namespace Nanocore.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct HKey
    {
        public UIntPtr Value;

        public HKey(int value)
        {
            Value = new UIntPtr((uint)value);
        }

        public HKey(uint value)
        {
            Value = new UIntPtr(value);
        }
    }

    partial class AdvApi32
    {
        public enum RegistryValueType : uint
        {
            None,
            Sz,
            ExpandSz,
            Binary,
            DWord,
            DWordBigEndian,
            Link,
            MultiSz,
            ResourceList,
            FullResourceDescriptor,
            ResourceRequirementsList
        }

        [Flags]
        internal enum RegistryKeyAccess : uint
        {
            /// <summary>
            /// Required to query the values of a registry key.
            /// </summary>
            QueryValue = 1 << 0,
            /// <summary>
            /// Required to create, delete, or set a registry value.
            /// </summary>
            SetValue = 1 << 1,
            /// <summary>
            /// Required to create a subkey of a registry key.
            /// </summary>
            CreateSubKey = 1 << 2,
            /// <summary>
            /// Required to enumerate the subkeys of a registry key.
            /// </summary>
            EnumerateSubKeys = 1 << 3,
            /// <summary>
            /// Required to request change notifications for a registry key or for subkeys of a registry key.
            /// </summary>
            Notify = 1 << 4,
            /// <summary>
            /// Reserved for system use.
            /// </summary>
            CreateLink = 1 << 5,
            /// <summary>
            /// Indicated that an application on 64-bit Windows should operate on the 64-bit registry view. This flag is ignored by 32-bit Windows.
            /// This flag must be combined using the OR operator with the other flags in this enumeration that either query or access registry values.
            /// </summary>
            Wow6464Key = 1 << 8,
            /// <summary>
            /// Indicates that an application on 64-bit Windows should operate on the 32-bit registry view. This flag is ignored by 32-bit Windows.
            /// This flag must be combined using the OR operator with the other flags in this enumeration that either query or access registry values.
            /// </summary>
            Wow6432Key = 1 << 9,
            Standard = 1 << 17,
            Read = QueryValue | EnumerateSubKeys | Notify | Standard,
            Execute = Read,
            Write = SetValue | CreateSubKey | Standard
        }

        public delegate uint RegQueryValueExADelegate(HKey hKey,
                                                      [In, MarshalAs(UnmanagedType.LPStr)] string lpValueName,
                                                      uint lpReserved,
                                                      ref RegistryValueType lpType,
                                                      IntPtr lpData,
                                                      ref uint lpcbData);
        public delegate uint RegQueryValueExWDelegate(HKey hKey,
                                                      [In, MarshalAs(UnmanagedType.LPWStr)] string lpValueName,
                                                      uint lpReserved,
                                                      ref RegistryValueType lpType,
                                                      IntPtr lpData,
                                                      ref uint lpcbData);
        public delegate uint RegOpenKeyExWDelegate(HKey hKey,
                                                   [MarshalAs(UnmanagedType.LPWStr)] string lpSubKey,
                                                   uint ulOptions,
                                                   RegistryKeyAccess samDesired,
                                                   out HKey phkResult);
        public delegate uint RegCloseKeyDelegate(HKey hKey);

        public static RegQueryValueExADelegate RegQueryValueExA;
        public static RegQueryValueExWDelegate RegQueryValueExW;
        public static RegOpenKeyExWDelegate RegOpenKeyExW;
        public static RegCloseKeyDelegate RegCloseKey;
    }
}
