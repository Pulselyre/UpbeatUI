/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Collections.Generic;

namespace UpbeatUI
{
    public class ActionDeferrer : IDisposable
    {
        private Queue<Action> _queue;
        private Action _unlocker;

        public ActionDeferrer(Action<Action<Action>> locker, Action unlocker)
        {
            if (locker == null)
                throw new ArgumentNullException("locker action must be provided.");
            if (unlocker == null)
                throw new ArgumentNullException("unlocker action must be provided.");
            _queue = new Queue<Action>();
            _unlocker = unlocker;
            locker(Defer);
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
