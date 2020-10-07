using System;
using System.ComponentModel;

namespace UpbeatUI
{
    public interface IContext : INotifyPropertyChanged
    {
        event EventHandler CloseRequested;
        event EventHandler<ContextCreatedEventArgs> ContextCreated;

        void Close();
    }
}
