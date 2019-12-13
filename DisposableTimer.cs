using System;
using System.Timers;

namespace UpbeatUI
{
    public class DisposableTimer : IDisposable
    {
        private readonly Timer _timer;
        private readonly ElapsedEventHandler _handler;

        public DisposableTimer(TimeSpan interval, Action callback)
        {
            _timer = new Timer(interval.TotalSeconds);
            _handler = new ElapsedEventHandler((o, e) => callback());
            _timer.Elapsed += _handler;
            _timer.Start();
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Elapsed -= _handler;
            _timer.Dispose();
        }
    }
}
