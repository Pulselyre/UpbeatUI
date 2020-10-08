/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Timers;

namespace UpbeatUI.ViewModel
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
