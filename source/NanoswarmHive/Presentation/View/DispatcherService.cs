using NanoswarmHive.Presentation.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace NanoswarmHive.Presentation.View
{
    public class DispatcherService : IDispatcherService
    {
        private readonly Dispatcher _dispatcher;

        public DispatcherService(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        public static Task InvokeTask(Dispatcher dispatcher, Func<Task> task)
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            dispatcher.InvokeAsync(async () => { await task(); tcs.SetResult(0); });
            return tcs.Task;
        }

        public static Task<TResult> InvokeTask<TResult>(Dispatcher dispatcher, Func<Task<TResult>> task)
        {
            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
            dispatcher.InvokeAsync(async () => tcs.SetResult(await task()));
            return tcs.Task;
        }

        public void Invoke(Action callback)
        {
            if (CheckAccess())
            {
                callback();
            }
            else
            {
                _dispatcher.Invoke(callback);
            }
        }

        public TResult Invoke<TResult>(Func<TResult> callback)
        {
            return CheckAccess() ? callback() : _dispatcher.Invoke(callback);
        }

        public Task InvokeAsync(Action callback)
        {
            return _dispatcher.InvokeAsync(callback).Task;
        }

        public Task LowPriorityInvokeAsync(Action callback)
        {
            return _dispatcher.InvokeAsync(callback, DispatcherPriority.ApplicationIdle).Task;
        }

        public Task<TResult> InvokeAsync<TResult>(Func<TResult> callback)
        {
            return _dispatcher.InvokeAsync(callback).Task;
        }

        public Task InvokeTask(Func<Task> task)
        {
            return InvokeTask(_dispatcher, task);
        }

        public Task<TResult> InvokeTask<TResult>(Func<Task<TResult>> task)
        {
            return InvokeTask(_dispatcher, task);
        }

        public bool CheckAccess()
        {
            return Thread.CurrentThread == _dispatcher.Thread;
        }

        public void EnsureAccess(bool isDispatcherThread = true)
        {
            if (isDispatcherThread && Thread.CurrentThread != _dispatcher.Thread)
            {
                throw new InvalidOperationException("The current thread was expected to be the dispatcher thread.");
            }
            if (!isDispatcherThread && Thread.CurrentThread == _dispatcher.Thread)
            {
                throw new InvalidOperationException("The current thread was not expected to be the dispatcher thread.");
            }
        }
    }
}
