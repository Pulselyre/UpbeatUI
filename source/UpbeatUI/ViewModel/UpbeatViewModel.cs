/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;

namespace UpbeatUI.ViewModel
{
    /// <summary>
    /// Provides a base class for <see cref="IUpbeatViewModel"/>s that will placed on a <see cref="UpbeatStack"/>.
    /// </summary>
    [Obsolete("The SignalToClose method is being replaced with the IUpbeatService.SetCloseCallback method. The UpbeatStack will no longer require the IUpbeatViewModel interface and will automatically handle IDisposables. This base UpbeatViewModel class will be removed in UpbeatUI 3.0.")]
    public abstract class UpbeatViewModel : BaseViewModel, IUpbeatViewModel
    {
        public virtual void Dispose() { }

        [Obsolete("The SignalToClose method is being replaced with the IUpbeatService.SetCloseCallback method. This base UpbeatViewModel class will be removed in UpbeatUI 3.0.")]
        public virtual void SignalToClose(Action closeCallback)
            => closeCallback();
    }
}
