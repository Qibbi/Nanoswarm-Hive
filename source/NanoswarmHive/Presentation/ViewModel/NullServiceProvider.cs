using System;

namespace NanoswarmHive.Presentation.ViewModel
{
    internal class NullServiceProvider : IViewModelServiceProvider
    {
        public event EventHandler<ServiceRegistrationEventArgs> ServiceRegistered { add { } remove { } }
        public event EventHandler<ServiceRegistrationEventArgs> ServiceUnregistered { add { } remove { } }

        public void RegisterService(object service)
        {
            throw new InvalidOperationException($"Cannot register a service on {nameof(NullServiceProvider)}.");
        }

        public void UnregisterService(object service)
        {
            throw new InvalidOperationException($"Cannot unregister a service on {nameof(NullServiceProvider)}.");
        }

        public object TryGet(Type serviceType)
        {
            return null;
        }

        public T TryGet<T>() where T : class
        {
            return null;
        }

        public object Get(Type serviceType)
        {
            throw new InvalidOperationException("No serive matches given type.");
        }

        public T Get<T>() where T : class
        {
            throw new InvalidOperationException("No serive matches given type.");
        }
    }
}
