/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Windows;
using UpbeatUI.ViewModel;

namespace BasicUpbeatUISample.ViewModel
{
    // This extends BaseViewModel, which provides pre-written SetProperty and RaisePropertyChanged methods.
    internal class PopupViewModel : BaseViewModel, IDisposable
    {
        private readonly SharedTimer _sharedTimer;

        public PopupViewModel(
            // These are the parameters the parent used when opening this ViewModel. The IUpbeatService can inject the Parameters object into this constructor to pass initialization data or callbacks.
            Parameters parameters,
            // This is a shared singleton service.
            SharedTimer sharedTimer)
        {
            _ = parameters ?? throw new NullReferenceException(nameof(parameters));
            _sharedTimer = sharedTimer ?? throw new ArgumentNullException(nameof(sharedTimer));

            Message = parameters.Message;
            XPosition = parameters.Position.X;
            YPosition = parameters.Position.Y;

            _sharedTimer.Ticked += SharedTimerTicked;
        }

        public string Message { get; }
        public string SecondsElapsed => $"{_sharedTimer.ElapsedSeconds} Seconds";
        public double XPosition { get; }
        public double YPosition { get; }

        public void Dispose() =>
            _sharedTimer.Ticked -= SharedTimerTicked;

        private void SharedTimerTicked(object sender, EventArgs e) =>
            // Ensure that the PropertyChanged event is raised on the UI thread
            Application.Current.Dispatcher.Invoke(() => RaisePropertyChanged(nameof(SecondsElapsed)));

        // This nested Parameters class (full class name: "PopupViewModel.Parameters") is what other ViewModels will create instances of to tell the IUpbeatStack what type of child ViewModel to add to the stack.
        public class Parameters
        {
            public string Message { get; init; }
            public Point Position { get; init; } = new Point(0.5, 0.5);
        }
    }
}
