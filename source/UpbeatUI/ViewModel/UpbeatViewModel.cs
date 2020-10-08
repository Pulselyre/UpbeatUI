/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;

namespace UpbeatUI.ViewModel
{
    /// <summary>
    /// Provides a base class for IUpbeatViewModels that will placed on a UpbeatStack.
    /// </summary>
    public abstract class UpbeatViewModel : BaseViewModel, IUpbeatViewModel
    {
        public virtual void Dispose() { }

        public virtual void SignalToClose(Action closeCallback)
            => closeCallback();
    }
}
