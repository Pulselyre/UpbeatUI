/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Collections.Generic;

namespace UpbeatUI.ViewModel
{
    public partial class UpbeatStack
    {
        private class UpbeatServiceDeferrer : IDisposable
        {
            private readonly Queue<Action> _queue = new Queue<Action>();
            private readonly Action _unlocker;

            public UpbeatServiceDeferrer(UpbeatService configurationService)
            {
                if (configurationService == null)
                    throw new ArgumentNullException("unlocker action must be provided.");
                _unlocker = configurationService.Unlock;
                configurationService.Lock(Defer);
            }

            public void Dispose()
            {
                while (_queue.Count > 0)
                    _queue.Dequeue()();
                _unlocker();
            }

            private void Defer(Action action)
                => _queue.Enqueue(action);
        }
    }
}
