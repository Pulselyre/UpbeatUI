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
