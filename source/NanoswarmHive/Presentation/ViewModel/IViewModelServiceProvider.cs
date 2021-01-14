using System;

namespace NanoswarmHive.Presentation.ViewModel
{
    public interface IViewModelServiceProvider
    {
        event EventHandler<ServiceRegistrationEventArgs> ServiceRegistered;
        event EventHandler<ServiceRegistrationEventArgs> ServiceUnregistered;

        void RegisterService(object service);

        void UnregisterService(object service);

        object TryGet(Type serviceType);

        T TryGet<T>() where T : class;

        object Get(Type serviceType);

        T Get<T>() where T : class;
    }
}
