/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.ComponentModel;

namespace UpbeatUI.ViewModel
{
    /// <summary>
    /// Defines functionality for a class that can be placed on an UpbeatStack.
    /// </summary>
    public interface IUpbeatViewModel : IDisposable, INotifyPropertyChanged
    {
        /// <summary>
        /// Signals to the IUpbeatViewModel that the user or another object would like it to close.
        /// </summary>
        /// <param name="closeCallback">A callback that the IUpbeatViewModel can call when it is ready to close.</param>
        void SignalToClose(Action closeCallback);
    }
}
