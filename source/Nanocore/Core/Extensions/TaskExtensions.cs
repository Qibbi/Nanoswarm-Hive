using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Nanocore.Core.Extensions
{
    public static class TaskExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Forget(this Task task)
        {
            Debug.Assert(!(task is null), $"{nameof(task)} is null");
        }
    }
}
