using System;

namespace NanoswarmHive.Presentation.ViewModel
{
    public class ServiceRegistrationEventArgs : EventArgs
    {
        public object Service { get; }

        public ServiceRegistrationEventArgs(object service)
        {
            Service = service;
        }
    }
}
