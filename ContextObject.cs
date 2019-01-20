using System;

namespace UpbeatUI
{
    public abstract class ContextObject : ObservableObject, IContext
    {
        public virtual void Dispose() { }

        public virtual void SignalToClose(Action closeCallback)
            => closeCallback();
    }
}
