/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;

namespace UpbeatUI.ViewModel
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
