using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace UpbeatUI
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        private static readonly Dictionary<string, PropertyChangedEventArgs> _eventArgCache = new Dictionary<string, PropertyChangedEventArgs>();

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler PositionsRequryRequested;

        public static PropertyChangedEventArgs GetPropertyChangedEventArgs(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException(
                    "propertyName cannot be null or empty.");

            PropertyChangedEventArgs e;
            lock (_eventArgCache)
            {
                if (!_eventArgCache.ContainsKey(propertyName))
                    _eventArgCache.Add(propertyName, new PropertyChangedEventArgs(propertyName));
                e = _eventArgCache[propertyName];
            }

            return e;
        }

        protected void RaisePropertyChanged(params string[] values)
        {
            foreach (string value in values)
                RaisePropertyChanged(value);
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                PropertyChangedEventArgs e = GetPropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        protected void RequestPositionsRequery()
            => PositionsRequryRequested?.Invoke(this, EventArgs.Empty);
    }
}
