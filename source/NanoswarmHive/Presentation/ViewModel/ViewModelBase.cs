using Nanocore.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NanoswarmHive.Presentation.ViewModel
{
    public abstract class AViewModelBase : INotifyPropertyChanging, INotifyPropertyChanged, IDestroyable
    {
        protected readonly Dictionary<string, string[]> DependentProperties = new Dictionary<string, string[]>();

        protected bool HasPropertyChangingSubscriber => !(PropertyChanging is null);
        protected bool HasPropertyChangedSubscriber => !(PropertyChanged is null);

        protected bool IsDestroyed { get; private set; }

        public IViewModelServiceProvider ServiceProvider { get; protected set; } = ViewModelServiceProvider.NullServiceProvider;

        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        protected AViewModelBase()
        {
        }

        protected AViewModelBase(IViewModelServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        protected void EnsureNotDestroyed(string name = null)
        {
            if (IsDestroyed)
            {
                throw new ObjectDisposedException(name ?? GetType().Name, "This view model has already been disposed.");
            }
        }

        protected bool SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            return SetValue(ref field, value, null, new[] { propertyName });
        }

        protected bool SetValue<T>(ref T field, T value, params string[] propertyNames)
        {
            return SetValue(ref field, value, null, propertyNames);
        }

        protected bool SetValue<T>(ref T field, T value, Action updateAction, [CallerMemberName] string propertyName = null)
        {
            return SetValue(ref field, value, updateAction, new[] { propertyName });
        }

        protected virtual bool SetValue<T>(ref T field, T value, Action updateAction, params string[] propertyNames)
        {
            if (propertyNames.Length == 0) throw new ArgumentOutOfRangeException(nameof(propertyNames), "This method must be invoked with at least one property name.");
            if (EqualityComparer<T>.Default.Equals(field, value) == false)
            {
                OnPropertyChanging(propertyNames);
                field = value;
                updateAction?.Invoke();
                OnPropertyChanged(propertyNames);
                return true;
            }
            return false;
        }

        protected bool SetValue(Action updateAction, [CallerMemberName] string propertyName = null)
        {
            return SetValue(null, updateAction, new[] { propertyName });
        }

        protected bool SetValue(Action updateAction, params string[] propertyNames)
        {
            return SetValue(null, updateAction, propertyNames);
        }

        protected bool SetValue(Func<bool> hasChangedFunction, Action updateAction, [CallerMemberName] string propertyName = null)
        {
            return SetValue(hasChangedFunction, updateAction, new[] { propertyName });
        }

        protected bool SetValue(bool hasChanged, Action updateAction, [CallerMemberName] string propertyName = null)
        {
            return SetValue(() => hasChanged, updateAction, new[] { propertyName });
        }

        protected bool SetValue(bool hasChanged, Action updateAction, params string[] propertyNames)
        {
            return SetValue(() => hasChanged, updateAction, propertyNames);
        }

        protected virtual bool SetValue(Func<bool> hasChangedFunction, Action updateAction, params string[] propertyNames)
        {
            if (propertyNames.Length == 0) throw new ArgumentOutOfRangeException(nameof(propertyNames), "This method must be invoked with at least one property name.");
            bool hasChanged = true;
            if (!(hasChangedFunction is null))
            {
                hasChanged = hasChangedFunction();
            }
            if (hasChanged)
            {
                OnPropertyChanging(propertyNames);
                updateAction?.Invoke();
                OnPropertyChanged(propertyNames);
            }
            return hasChanged;
        }

        protected virtual void OnPropertyChanging(params string[] propertyNames)
        {
            PropertyChangingEventHandler propertyChanging = PropertyChanging;
            foreach (string propertyName in propertyNames)
            {
                propertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
                if (DependentProperties.TryGetValue(propertyName, out string[] dependentProperties))
                {
                    OnPropertyChanging(dependentProperties);
                }
            }
        }

        protected virtual void OnPropertyChanged(params string[] propertyNames)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            foreach (string propertyName in propertyNames.Reverse())
            {
                if (DependentProperties.TryGetValue(propertyName, out string[] dependentProperties))
                {
                    OnPropertyChanged(dependentProperties.Reverse().ToArray());
                }
                propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public virtual void Destroy()
        {
            IsDestroyed = true;
        }
    }
}
