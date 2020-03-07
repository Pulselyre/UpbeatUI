/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace UpbeatUI.Context
{
    /// <summary>
    /// Provides a base class with convenience methods for View Models.
    /// </summary>
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        private static readonly Dictionary<string, PropertyChangedEventArgs> _eventArgCache = new Dictionary<string, PropertyChangedEventArgs>();

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Returns a PropertyChangedEventArgs containing propertyName.
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        /// <returns>A PropertyChangedEventArgs containing propertyName</returns>
        public static PropertyChangedEventArgs GetPropertyChangedEventArgs(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException("propertyName cannot be null or empty.");
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
        /// Raises PropertyChanged events for each property name in propertyNames.
        /// </summary>
        /// <param name="propertyNames">The names of the properties</param>
        protected void RaisePropertyChanged(params string[] propertyNames)
        {
            foreach (string value in propertyNames)
                RaisePropertyChanged(value);
        }

        /// <summary>
        /// Raises a PropertyChanged event for propertyName.
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        protected void RaisePropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, GetPropertyChangedEventArgs(propertyName));
    }
}
