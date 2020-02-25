/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;

namespace UpbeatUI.Context
{
    public abstract class ContextObject : ObservableObject, IContext
    {
        public virtual void Dispose() { }

        public virtual void SignalToClose(Action closeCallback)
            => closeCallback();
    }
}
