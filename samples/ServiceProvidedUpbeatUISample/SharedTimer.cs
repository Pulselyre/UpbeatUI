/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Timers;

namespace ServiceProvidedUpbeatUISample
{
    // This is a simple timer to demonstrate a singleton service shared between multiple ViewModels.
    public class SharedTimer : IDisposable
    {
        private readonly Timer _timer;

        public SharedTimer()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += TimerElapsed;
            _timer.Start();
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            ElapsedSeconds++;
            Ticked?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Ticked;

        public int ElapsedSeconds { get; private set; }

        public void Dispose()
        {
            _timer.Elapsed -= TimerElapsed;
            _timer.Stop();
        }
    }
}
