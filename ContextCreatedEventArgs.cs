using System;

namespace UpbeatUI
{
    public class ContextCreatedEventArgs : EventArgs
    {
        public ContextCreatedEventArgs(IContext context)
        {
            Context = context;
        }

        public IContext Context { get; }
    }
}
