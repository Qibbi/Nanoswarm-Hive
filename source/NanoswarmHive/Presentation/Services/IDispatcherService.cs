using System;
using System.Threading.Tasks;

namespace NanoswarmHive.Presentation.Services
{
    public interface IDispatcherService
    {
        void Invoke(Action callback);

        TResult Invoke<TResult>(Func<TResult> callback);

        Task InvokeAsync(Action callback);

        Task LowPriorityInvokeAsync(Action callback);

        Task<TResult> InvokeAsync<TResult>(Func<TResult> callback);

        Task InvokeTask(Func<Task> task);

        Task<TResult> InvokeTask<TResult>(Func<Task<TResult>> task);

        bool CheckAccess();

        void EnsureAccess(bool isDispatcherThread = true);
    }
}
