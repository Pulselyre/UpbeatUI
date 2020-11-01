/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.ComponentModel;

namespace UpbeatUI.ViewModel
{
    /// <summary>
    /// Defines functionality for a class that can be placed on an <see cref="UpbeatStack"/>.
    /// </summary>
    [Obsolete("The SignalToClose method is being replaced with the IUpbeatService.SetCloseCallback method. The UpbeatStack will no longer require the IUpbeatViewModel interface and will automatically handle IDisposables. This IUpbeatViewModel interface will be removed in UpbeatUI 3.0.")]
    public interface IUpbeatViewModel : IDisposable, INotifyPropertyChanged
    {
        /// <summary>
        /// Signals to the <see cref="IUpbeatViewModel"/> that the user or another object would like it to close.
        /// </summary>
        /// <param name="closeCallback">A delegate that the <see cref="IUpbeatViewModel"/> can call when it is ready to close.</param>
        [Obsolete("The SignalToClose method is being replaced with the IUpbeatService.SetCloseCallback method. This base UpbeatViewModel class will be removed in UpbeatUI 3.0.")]
        void SignalToClose(Action closeCallback);
    }
}
