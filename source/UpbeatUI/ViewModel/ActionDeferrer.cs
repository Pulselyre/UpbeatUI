/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Collections.Generic;

namespace UpbeatUI.ViewModel
{
    /// <summary>
    /// Provides functionality to queue up <see cref="Action"/>s and execute them upon disposal.
    /// </summary>
    public class ActionDeferrer : IDisposable
    {
        private Queue<Action> _queue = new Queue<Action>();
        private Action _unlocker;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionDeferrer"/> class, passes a <see cref="Defer(Action)"/> method to the caller using the <<paramref name="locker"/> callback, and saves the <paramref name="unlocker"/> callback to be executed on disposal.
        /// </summary>
        /// <param name="locker">A callback to receiver the <see cref="ActionDeferrer"/>'s defer delegate.</param>
        /// <param name="unlocker">A callback for the <see cref="ActionDeferrer"/> to execute when the queue has been emptied after disposal.</param>
        public ActionDeferrer(Action<Action<Action>> locker, Action unlocker)
        {
            if (locker == null)
                throw new ArgumentNullException("locker action must be provided.");
            if (unlocker == null)
                throw new ArgumentNullException("unlocker action must be provided.");
            _unlocker = unlocker;
            locker(Defer);
        }

        /// <summary>
        /// Executes all items in the queue, then calls the unlocker callback.
        /// </summary>
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
