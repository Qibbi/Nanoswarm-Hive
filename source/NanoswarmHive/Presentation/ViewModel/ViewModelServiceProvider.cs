using System;
using System.Collections.Generic;
using System.Linq;

namespace NanoswarmHive.Presentation.ViewModel
{
    public class ViewModelServiceProvider : IViewModelServiceProvider
    {
        public static readonly IViewModelServiceProvider NullServiceProvider = new NullServiceProvider();

        private readonly IViewModelServiceProvider _parentProvider;
        private readonly List<object> _services = new List<object>();

        public event EventHandler<ServiceRegistrationEventArgs> ServiceRegistered;
        public event EventHandler<ServiceRegistrationEventArgs> ServiceUnregistered;

        public ViewModelServiceProvider(IEnumerable<object> services = null) : this(null, services)
        {
        }

        public ViewModelServiceProvider(IViewModelServiceProvider parentProvider, IEnumerable<object> services = null)
        {
            if (parentProvider is ViewModelServiceProvider parent)
            {
                foreach (object service in parent._services)
                {
                    RegisterService(service);
                }
            }
            else
            {
                _parentProvider = parentProvider;
            }
            if (services is null)
            {
                return;
            }
            foreach (object service in services)
            {
                RegisterService(service);
            }
        }

        public void RegisterService(object service)
        {
            if (service is null) throw new ArgumentNullException(nameof(service));
            if (_services.Any(x => x.GetType() == service.GetType()))
            {
                throw new InvalidOperationException($"A service of type '{service.GetType().Name}' has already been registered.");
            }
            _services.Add(service);
            ServiceRegistered?.Invoke(this, new ServiceRegistrationEventArgs(service));
        }

        public void UnregisterService(object service)
        {
            if (service is null) throw new ArgumentNullException(nameof(service));
            _services.Remove(service);
            ServiceUnregistered?.Invoke(this, new ServiceRegistrationEventArgs(service));
        }

        public object TryGet(Type serviceType)
        {
            if (serviceType is null) throw new ArgumentNullException(nameof(serviceType));
            object foundService = null;
            foreach (object service in _services.Where(serviceType.IsInstanceOfType))
            {
                if (foundService is null)
                {
                    foundService = service;
                }
                else
                {
                    throw new InvalidOperationException($"Multiple services match the type '{serviceType.Name}'.");
                }
            }
            return foundService ?? _parentProvider?.TryGet(serviceType);
        }

        public T TryGet<T>() where T : class
        {
            return (T)TryGet(typeof(T));
        }

        public object Get(Type serviceType)
        {
            object result = TryGet(serviceType);
            if (result is null) throw new InvalidOperationException($"No services match the type '{serviceType.Name}'.");
            return result;
        }

        public T Get<T>() where T : class
        {
            T result = (T)TryGet(typeof(T));
            if (result is null) throw new InvalidOperationException($"No services match the type '{typeof(T).Name}'.");
            return result;
        }
    }
}
