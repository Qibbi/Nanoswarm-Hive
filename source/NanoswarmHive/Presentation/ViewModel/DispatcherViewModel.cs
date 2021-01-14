using NanoswarmHive.Presentation.Services;
using System;

namespace NanoswarmHive.Presentation.ViewModel
{
    public abstract class ADispatcherViewModel : AViewModelBase
    {
        public IDispatcherService Dispatcher { get; }

        protected ADispatcherViewModel(IViewModelServiceProvider serviceProvider) : base(serviceProvider)
        {
            Dispatcher = serviceProvider.TryGet<IDispatcherService>();
            if (Dispatcher is null) throw new InvalidOperationException($"{nameof(ADispatcherViewModel)} requires a {nameof(IDispatcherService)} in the service provider.");
        }

        protected override void OnPropertyChanging(params string[] propertyNames)
        {
            if (HasPropertyChangingSubscriber)
            {
                Dispatcher.Invoke(() => base.OnPropertyChanging(propertyNames));
            }
            else
            {
                base.OnPropertyChanging(propertyNames);
            }
        }

        protected override void OnPropertyChanged(params string[] propertyNames)
        {
            if (HasPropertyChangingSubscriber)
            {
                Dispatcher.Invoke(() => base.OnPropertyChanged(propertyNames));
            }
            else
            {
                base.OnPropertyChanged(propertyNames);
            }
        }
    }
}
