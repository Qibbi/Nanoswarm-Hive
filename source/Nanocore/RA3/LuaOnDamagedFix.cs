using Nanocore.Core.Diagnostics;
using System;
using System.Runtime.InteropServices;

namespace Nanocore.RA3
{
    [Hook]
    public static class LuaOnDamagedFix
    {
        private static readonly Tracer _tracer = Tracer.GetTracer(nameof(LuaOnDamagedFix), "Fixes lua OnDamage event");

        private delegate void StringFormatDelegate(ref IntPtr destination, IntPtr strFormat, int param);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate void StringReleaseBufferDelegate(ref IntPtr buffer);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void LuaPushNilDelegate(IntPtr luaState);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void LuaGetGlobalDelegate(IntPtr luaState, [In] IntPtr name);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int LuaGetTopDelegate(IntPtr luaState);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void LuaSetTopDelegate(IntPtr luaState, int index);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int LuaGetTypeDelegate(IntPtr luaState, int index);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int LuaGetIdDelegate(IntPtr luaState, int index);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate void PushGameObjectDelegate(IntPtr hInstance, IntPtr luaState, IntPtr gameObject);

        [GameFunction(Origin = 0x004CC940, Steam = 0x004CC5C0)]
        private static readonly StringFormatDelegate _stringFormat;

        [GameFunction(Origin = 0x004CB3B0, Steam = 0x004CB050)]
        private static readonly StringReleaseBufferDelegate _stringReleaseBuffer;

        [GameFunction(Origin = 0x0040A8D0, Steam = 0x0040A8B0)]
        private static readonly LuaPushNilDelegate _luaPushNil;

        [GameFunction(Origin = 0x0040AA70, Steam = 0x0040AA50)]
        private static readonly LuaGetGlobalDelegate _luaGetGlobal;

        [GameFunction(Origin = 0x0040A3F0, Steam = 0x0040A3D0)]
        private static readonly LuaGetTopDelegate _luaGetTop;

        [GameFunction(Origin = 0x0040A400, Steam = 0x0040A3E0)]
        private static readonly LuaSetTopDelegate _luaSetTop;

        [GameFunction(Origin = 0x0040A540, Steam = 0x0040A520)]
        private static readonly LuaGetTypeDelegate _luaGetType;

        [GameFunction(Origin = 0x0040A5F0, Steam = 0x0040A5D0)]
        private static readonly LuaGetIdDelegate _luaGetId;

        [GamePointer(Origin = 0x00BDF97C, Steam = 0x00BDF984)]
        private static readonly IntPtr _theNullChr;

        [HookFunction(nameof(PushGameObject), Origin = 0x005AFEA0, Steam = 0x0056E7F0)]
        private static readonly PushGameObjectDelegate _pushGameObject;

        private static readonly IntPtr _objIdFormat;

        static LuaOnDamagedFix()
        {
            _objIdFormat = Marshal.StringToHGlobalAnsi("ObjID#%08x");
        }

        private static void PushGameObject(IntPtr hInstance, IntPtr luaState, IntPtr gameObject)
        {
            IntPtr state = Marshal.ReadIntPtr(hInstance + 0x24);
            if (gameObject == IntPtr.Zero)
            {
                _luaPushNil(state);
            }
            else
            {
                int objectId = Marshal.ReadInt32(gameObject + 0xDC);
                _tracer.TraceNote("Got object id {0:X8}", objectId);
                IntPtr str = IntPtr.Zero;
                _stringFormat(ref str, _objIdFormat, objectId);
                if (str == IntPtr.Zero)
                {
                    str = _theNullChr;
                }
                _luaGetGlobal(state, str);
                int top = _luaGetTop(luaState);
                _tracer.TraceNote("Stack size is {0}", top);
                int checkType = -1;
                int checkId = -1;
                if ((checkType = _luaGetType(state, top)) == 1 || objectId != (checkId = _luaGetId(state, top)))
                {
                    _tracer.TraceNote("Check failed with {0} {1:X8}", checkType, checkId);
                    _luaSetTop(state, top - 1);
                    _luaPushNil(state);
                }
                else
                {
                    _tracer.TraceNote("Check succeeded with {0} {1:X8}", checkType, checkId);
                }
                _stringReleaseBuffer(ref str);
            }
        }
    }
}
