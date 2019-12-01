using System;
using System.Threading;

namespace UpbeatUI
{
    public class DisposableThread : IDisposable
    {
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private ManualResetEvent _resetEvent = new ManualResetEvent(false);

        public DisposableThread(Action<CancellationToken> threadStart)
        {
            var thread = new Thread(
                () =>
                {
                    threadStart(_cancellationTokenSource.Token);
                    _resetEvent.Set();
                });
            thread.Start();
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _resetEvent.WaitOne();
        }
    }
}
