using Nanocore.Core.Diagnostics;
using Nanocore.Core.Extensions;
using Nanocore.Native;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace Nanocore
{
    internal static class CncOnline
    {
        private static readonly Tracer _tracer = Tracer.GetTracer(nameof(CncOnline), "Allows connection to cnc online.");
        #region PublicKeys
        private static readonly byte[] _eaPublicKey = new byte[] { 0x92, 0x75, 0xA1, 0x5B, 0x08, 0x02, 0x40, 0xB8,
                                                                   0x9B, 0x40, 0x2F, 0xD5, 0x9C, 0x71, 0xC4, 0x51,
                                                                   0x58, 0x71, 0xD8, 0xF0, 0x2D, 0x93, 0x7F, 0xD3,
                                                                   0x0C, 0x8B, 0x1C, 0x7D, 0xF9, 0x2A, 0x04, 0x86,
                                                                   0xF1, 0x90, 0xD1, 0x31, 0x0A, 0xCB, 0xD8, 0xD4,
                                                                   0x14, 0x12, 0x90, 0x3B, 0x35, 0x6A, 0x06, 0x51,
                                                                   0x49, 0x4C, 0xC5, 0x75, 0xEE, 0x0A, 0x46, 0x29,
                                                                   0x80, 0xF0, 0xD5, 0x3A, 0x51, 0xBA, 0x5D, 0x6A,
                                                                   0x19, 0x37, 0x33, 0x43, 0x68, 0x25, 0x2D, 0xFE,
                                                                   0xDF, 0x95, 0x26, 0x36, 0x7C, 0x43, 0x64, 0xF1,
                                                                   0x56, 0x17, 0x0E, 0xF1, 0x67, 0xD5, 0x69, 0x54,
                                                                   0x20, 0xFB, 0x3A, 0x55, 0x93, 0x5D, 0xD4, 0x97,
                                                                   0xBC, 0x3A, 0xD5, 0x8F, 0xD2, 0x44, 0xC5, 0x9A,
                                                                   0xFF, 0xCD, 0x0C, 0x31, 0xDB, 0x9D, 0x94, 0x7C,
                                                                   0xA6, 0x66, 0x66, 0xFB, 0x4B, 0xA7, 0x5E, 0xF8,
                                                                   0x64, 0x4E, 0x28, 0xB1, 0xA6, 0xB8, 0x73, 0x95  };

        private static readonly byte[] _cncOnlinePublicKey = new byte[] { 0xE7, 0xCD, 0xB7, 0xD8, 0x15, 0x51, 0xC5, 0xD7,
                                                                          0xED, 0xBE, 0x0D, 0x59, 0x6A, 0xCC, 0x45, 0x72,
                                                                          0x03, 0xFB, 0xE0, 0x08, 0x5A, 0x7C, 0x75, 0xBC,
                                                                          0xBF, 0x50, 0x7A, 0x24, 0xE2, 0x5A, 0x4A, 0x71,
                                                                          0x31, 0x63, 0x07, 0xD8, 0xDE, 0x03, 0x96, 0xF6,
                                                                          0x84, 0x18, 0x1A, 0xAB, 0xC9, 0x55, 0x4B, 0x4F,
                                                                          0x87, 0x9F, 0x43, 0x63, 0x43, 0x72, 0xBF, 0x6F,
                                                                          0xF0, 0x72, 0x66, 0x95, 0x3A, 0x63, 0xE5, 0x41,
                                                                          0xAE, 0x6F, 0x58, 0xD9, 0xCF, 0x23, 0xFA, 0x49,
                                                                          0x0B, 0x3C, 0xEF, 0x28, 0xE5, 0x51, 0xF6, 0x99,
                                                                          0x37, 0x51, 0xE7, 0xEF, 0x43, 0x12, 0xB4, 0x1B,
                                                                          0x59, 0x31, 0x5B, 0x55, 0x05, 0x51, 0x0C, 0x3C,
                                                                          0xC0, 0x51, 0x28, 0x43, 0x28, 0xFB, 0x00, 0x51,
                                                                          0x74, 0x7C, 0x2C, 0xDA, 0x5A, 0x84, 0xDE, 0xC1,
                                                                          0xD0, 0x40, 0x26, 0x32, 0xC1, 0xA1, 0x80, 0xAD,
                                                                          0x0D, 0x07, 0xE5, 0xAD, 0x93, 0x0D, 0x2D, 0xF5};
        #endregion
        private static readonly string _updateServer = "http.server.cnc-online.net"; // TODO: make this a setting

        public static unsafe bool ModifyPublicKey(System.Diagnostics.Process process)
        {
            System.Diagnostics.ProcessModule module = process.MainModule;
            int index;
            int oldProtection = 0;
            if (!Kernel32.VirtualProtect(module.BaseAddress, module.ModuleMemorySize, 0x40, ref oldProtection))
            {
                _tracer.TraceException(new Win32Exception(Marshal.GetLastWin32Error()).Message);
                return false;
            }
            byte* pCurrentModule = (byte*)module.BaseAddress.ToPointer();
            using (Stream stream = new UnmanagedMemoryStream(pCurrentModule, module.ModuleMemorySize, module.ModuleMemorySize, FileAccess.ReadWrite))
            {
                byte[] buffer = new byte[stream.Length];
                if (stream.Read(buffer, 0, buffer.Length) != buffer.Length)
                {
                    _tracer.TraceException("Error reading module.");
                    return false;
                }
                index = buffer.IndexOf(_eaPublicKey);
                if (index < 0)
                {
                    _tracer.TraceException("Error finding public key.");
                    index = buffer.IndexOf(_cncOnlinePublicKey);
                    _tracer.TraceInfo("CnC Online Public Key already set.");
                    return false;
                }
                _tracer.TraceInfo($"Public key found at 0x{index:X08}.");
                stream.Position = index;
                stream.Write(_cncOnlinePublicKey, 0, _cncOnlinePublicKey.Length);
            }
            return true;
        }

        public static unsafe IntPtr GetHostByName([In] string host)
        {
            _tracer.TraceInfo($"Got host request to: '{host}'.");
            IntPtr result = IntPtr.Zero;
            switch (host)
            {
                case "servserv.generals.ea.com":
                case "na.llnet.eadownloads.ea.com":
                    host = _updateServer;
                    _tracer.TraceInfo($"Redirecting to: '{host}'.");
                    break;
                case "bfme.fesl.ea.com":
                case "bfme2.fesl.ea.com":
                case "bfme2-ep1-pc.fesl.ea.com":
                case "cnc3-pc.fesl.ea.com":
                case "cnc3-ep1-pc.fesl.ea.com":
                case "cncra3-pc.fesl.ea.com":
                    host = "login.server.cnc-online.net";
                    _tracer.TraceInfo($"Redirecting to: '{host}'.");
                    break;
                case "gpcm.gamespy.com":
                    host = "gpcm.server.cnc-online.net";
                    _tracer.TraceInfo($"Redirecting to: '{host}'.");
                    break;
                case "peerchat.gamespy.com":
                    host = "peerchat.server.cnc-online.net";
                    _tracer.TraceInfo($"Redirecting to: '{host}'.");
                    break;
                case "lotrbme.available.gamespy.com":
                case "lotrbme.master.gamespy.com":
                case "lotrbme.ms13.gamespy.com":
                case "lotrbme2r.available.gamespy.com":
                case "lotrbme2r.master.gamespy.com":
                case "lotrbme2r.ms9.gamespy.com":
                case "ccgenerals.ms19.gamespy.com":
                case "ccgenzh.ms6.gamespy.com":
                case "cc3tibwars.available.gamespy.com":
                case "cc3tibwars.master.gamespy.com":
                case "cc3tibwars.ms17.gamespy.com":
                case "cc3xp1.available.gamespy.com":
                case "cc3xp1.master.gamespy.com":
                case "cc3xp1.ms18.gamespy.com":
                case "redalert3pc.available.gamespy.com":
                case "redalert3pc.master.gamespy.com":
                case "redalert3pc.ms1.gamespy.com":
                case "master.gamespy.com":
                    host = "master.server.cnc-online.net";
                    _tracer.TraceInfo($"Redirecting to: '{host}'.");
                    break;
                case "redalert3pc.natneg1.gamespy.com":
                case "redalert3pc.natneg2.gamespy.com":
                case "redalert3pc.natneg3.gamespy.com":
                    host = "natneg.server.cnc-online.net";
                    _tracer.TraceInfo($"Redirecting to: '{host}'.");
                    break;
                case "lotrbme.gamestats.gamespy.com":
                case "lotrbme2r.gamestats.gamespy.com":
                case "gamestats.gamespy.com":
                    host = "gamestats.server.cnc-online.net";
                    _tracer.TraceInfo($"Redirecting to: '{host}'.");
                    break;
                case "cc3tibwars.auth.pubsvs.gamespy.com":
                case "cc3tibwars.comp.pubsvs.gamespy.com":
                case "cc3tibwars.sake.gamespy.com":
                case "cc3xp1.auth.pubsvs.gamespy.com":
                case "cc3xp1.comp.pubsvs.gamespy.com":
                case "cc3xp1.sake.gamespy.com":
                case "redalert3pc.auth.pubsvs.gamespy.com":
                case "redalert3pc.comp.pubsvs.gamespy.com":
                case "redalert3pc.sake.gamespy.com":
                case "redalert3services.gamespy.com":
                case "psweb.gamespy.com":
                    host = "sake.server.cnc-online.net";
                    _tracer.TraceInfo($"Redirecting to: '{host}'.");
                    break;
                case "lotrbfme.arenasdk.gamespy.com":
                case "arenasdk.gamespy.com":
                case "launch.gamespyarcade.com":
                case "www.gamespy.com":
                case "ingamead.gamespy.com":
                    host = "server.cnc-online.net";
                    _tracer.TraceInfo($"Redirecting to: '{host}'.");
                    break;
            }
            return Ws2_32.GetHostByName(host);
        }

        public static int Send(IntPtr s, IntPtr buf, int len, int flags)
        {
            string str = Marshal.PtrToStringAnsi(buf, len);
            if (str.StartsWith("GET ") || str.StartsWith("HEAD "))
            {
                // str = str.Replace("/servserv/cnc3/", "/tsrgame/servserv/");
                // str = str.Replace("Host: servserv.generals.ea.com", "Host: rising.cnc-source.com");
                str = str.Replace("Host: na.llnet.eadownloads.ea.com", $"Host: {_updateServer}");
                _tracer.TraceNote($"Sending '{str}'.");
                IntPtr pStr = Marshal.StringToHGlobalAnsi(str);
                int result = Ws2_32.Send(s, pStr, str.Length, flags);
                Marshal.FreeHGlobal(pStr);
                return result;
            }
            _tracer.TraceNote($"{nameof(Send)} from 0x{EasyHook.HookRuntimeInfo.ReturnAddress.ToInt32():X8}");
            // _tracer.TraceNote($"Sending unmodified '{str}'.");
            return Ws2_32.Send(s, buf, len, flags);
        }
    }
}
