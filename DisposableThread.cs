using System;
using System.Threading;

namespace UpbeatUI
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
