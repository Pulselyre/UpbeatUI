using System;

namespace UpbeatUI
{
    public partial class ContextStack : ObservableObject, IDisposable, IUpdatableContext
    {
        private class ContextServiceDeferrer : ActionDeferrer
        {
            public ContextServiceDeferrer(ContextService configurationService)
                : base(configurationService.Lock, configurationService.Unlock)
            { }
        }
    }
}
