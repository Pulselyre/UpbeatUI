/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System.Collections.Generic;

namespace UpbeatUI.ViewModel
{
    public class ObjectNameViewModel<T> : BaseViewModel
    {
        private KeyValuePair<T, string> _keyValuePair;

        public ObjectNameViewModel(T target, string name)
        {
            Synchronize(new KeyValuePair<T, string>(target, name));
        }

        public ObjectNameViewModel(KeyValuePair<T, string> kvp)
        {
            Synchronize(kvp);
        }

        public string Name { get { return _keyValuePair.Value; } }
        public T Target { get { return _keyValuePair.Key; } }

        public void Synchronize(KeyValuePair<T, string> newModelObject)
        {
            _keyValuePair = newModelObject;
            RaisePropertyChanged(nameof(Name), nameof(Target));
        }

        public void Synchronize(T target, string name)
            => Synchronize(new KeyValuePair<T, string>(target, name));
    }
}
