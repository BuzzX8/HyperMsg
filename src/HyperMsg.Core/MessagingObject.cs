using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HyperMsg
{
    public class MessagingObject : MessagingContextProxy, INotifyPropertyChanged, IDisposable
    {
        private readonly List<IDisposable> registrations = new();

        public MessagingObject(IMessagingContext messagingContext) : base(messagingContext)
        { }

        protected void AddRegistration(IDisposable registration) => registrations.Add(registration);

        protected void AddRegistrations(params IDisposable[] registrations) => this.registrations.AddRange(registrations);

        protected void AddRegistrations(IEnumerable<IDisposable> registrations) => this.registrations.AddRange(registrations);

        public virtual void Dispose() => registrations.ForEach(r => r.Dispose());

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
