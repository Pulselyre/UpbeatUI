/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;

namespace UpbeatUI.Extensions.Hosting
{
    internal sealed class UpbeatApplicationService : IUpbeatApplicationService
    {
        internal event EventHandler CloseRequested;

        public void CloseUpbeatApplication() => CloseRequested?.Invoke(this, EventArgs.Empty);
    }
}
