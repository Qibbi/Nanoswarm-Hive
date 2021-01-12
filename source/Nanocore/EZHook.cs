using EasyHook;

namespace Nanocore
{
    public static class EZHook
    {
        public static void Inject(int processId, string library, params object[] args)
        {
            RemoteHooking.Inject(processId, library, library, args);
        }
    }
}
