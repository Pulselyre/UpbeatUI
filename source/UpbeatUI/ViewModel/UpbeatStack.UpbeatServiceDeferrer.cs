/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;

namespace UpbeatUI.ViewModel
{
    public partial class UpbeatStack : BaseViewModel, IDisposable
    {
        private class UpbeatServiceDeferrer : ActionDeferrer
        {
            public UpbeatServiceDeferrer(UpbeatService configurationService)
                : base(configurationService.Lock, configurationService.Unlock)
            { }
        }
    }
}
