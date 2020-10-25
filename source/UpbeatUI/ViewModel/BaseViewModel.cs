/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UpbeatUI.ViewModel
{
    /// <summary>
    /// Provides a base class with convenience methods for ViewModels.
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        private static readonly Dictionary<string, PropertyChangedEventArgs> _eventArgCache = new Dictionary<string, PropertyChangedEventArgs>();

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Returns a <see cref="PropertyChangedEventArgs"/> containing <paramref name="propertyName"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        /// <returns>A <see cref="PropertyChangedEventArgs"/> containing <paramref name="propertyName"/></returns>
        public static PropertyChangedEventArgs GetPropertyChangedEventArgs(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException($"{nameof(propertyName)} cannot be null or empty.");
            PropertyChangedEventArgs e;
            lock (_eventArgCache)
            {
                if (!_eventArgCache.TryGetValue(propertyName, out e))
                {
                    e = new PropertyChangedEventArgs(propertyName);
                    _eventArgCache[propertyName] = e;
                }
            }
            return e;
        }

        /// <summary>
        /// Raises <see cref="PropertyChanged"/> events for each property name in <paramref name="propertyNames"/>.
        /// </summary>
        /// <param name="propertyNames">The names of the properties</param>
        protected void RaisePropertyChanged(params string[] propertyNames)
        {
            foreach (string value in propertyNames)
                RaisePropertyChanged(value);
        }

        /// <summary>
        /// Raises a <see cref="PropertyChanged"/> event for <paramref name="propertyName"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        protected void RaisePropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, GetPropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Sets a property's backing value and raises a <see cref="PropertyChanged"/> event, if necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property and backing value.</typeparam>
        /// <param name="backingValue">Reference to the backing value.</param>
        /// <param name="newValue">The desired new value.</param>
        /// <param name="propertyName">The name of the property (used in the <see cref="PropertyChanged"/> event). Optional, will be retrieved automatically using <see cref="CallerMemberNameAttribute"/>.</param>
        /// <returns>True if the newValue differed from the backingValue and a PropertyChanged needed to be raised; false otherwise.</returns>
        protected bool SetProperty<T>(ref T backingValue, T newValue, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingValue, newValue))
                return false;
            backingValue = newValue;
            RaisePropertyChanged(propertyName);
            return true;
        }
    }
}
