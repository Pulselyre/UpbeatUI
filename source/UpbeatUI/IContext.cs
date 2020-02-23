/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.ComponentModel;

namespace UpbeatUI
{
    public delegate IContext ContextCreator(IContextService contextService);

    public interface IContext : IDisposable, INotifyPropertyChanged
    {
        void SignalToClose(Action closeCallback);
    }
}
