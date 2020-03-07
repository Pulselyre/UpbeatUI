/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.ComponentModel;

namespace UpbeatUI.Context
{
    public delegate IContext ContextCreator(IContextService contextService);

    /// <summary>
    /// Defines functionality for a class that can be placed on a ContextStack.
    /// </summary>
    public interface IContext : IDisposable, INotifyPropertyChanged
    {
        /// <summary>
        /// Signals to the context that the user or another object would like it to close.
        /// </summary>
        /// <param name="closeCallback">A callback that the IContext can call when it is ready to close.</param>
        void SignalToClose(Action closeCallback);
    }
}
