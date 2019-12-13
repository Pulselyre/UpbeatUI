using System;
using System.Windows.Threading;

namespace UpbeatUI
{
    public class DisposableDispatcherTimer : IDisposable
    {
        private readonly DispatcherTimer _timer;
        private readonly EventHandler _handler;

        public DisposableDispatcherTimer(TimeSpan interval, Action callback)
            : this(interval, DispatcherPriority.Background, callback) { }

        public DisposableDispatcherTimer(TimeSpan interval, DispatcherPriority priority, Action callback)
        {
            _timer = new DispatcherTimer(priority)
            {
                Interval = interval,
            };
            _handler = new EventHandler((o, e) => callback());
            _timer.Tick += _handler;
            _timer.Start();
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Tick -= _handler;
        }
    }
}
