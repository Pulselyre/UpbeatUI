/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Threading;

namespace UpbeatUI.ViewModel
{
    public class DisposableThread : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly Thread _thread;

        public DisposableThread(Action<CancellationToken> threadStart)
            : this(threadStart, ThreadPriority.Normal) { }
        public DisposableThread(Action<CancellationToken> threadStart, ThreadPriority threadPriority)
        {
            _thread = new Thread(() => threadStart(_cancellationTokenSource.Token))
            {
                Priority = threadPriority
            };
            _thread.Start();
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _thread.Join();
            _cancellationTokenSource.Dispose();
        }
    }
}
