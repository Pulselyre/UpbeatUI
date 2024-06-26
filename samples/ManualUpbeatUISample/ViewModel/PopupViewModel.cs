/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ManualUpbeatUISample.ViewModel;

// This extends ObservableObject from the CommunityToolkit.Mvvm NuGet package, which provides pre-written SetProperty and OnPropertyChanged methods.
internal class PopupViewModel : ObservableObject, IDisposable
{
    private readonly SharedTimer _sharedTimer;
    private bool _disposed;

    public PopupViewModel(
        Parameters parameters, // These are the parameters the parent used when opening this ViewModel. The IUpbeatService can inject the Parameters object into this constructor to pass initialization data or callbacks.
        SharedTimer sharedTimer) // This is a shared singleton service.
    {
        _ = parameters ?? throw new ArgumentNullException(nameof(parameters));
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

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            _sharedTimer.Ticked -= SharedTimerTicked;
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~PopupViewModel() => Dispose(false);

    private void SharedTimerTicked(object sender, EventArgs e) =>
        Application.Current.Dispatcher.Invoke(() => OnPropertyChanged(nameof(SecondsElapsed))); // Ensure that the PropertyChanged event is raised on the UI thread

    // This nested Parameters class (full class name: "PopupViewModel.Parameters") is what other ViewModels will create instances of to tell the IUpbeatStack what type of child ViewModel to add to the stack.
    public class Parameters
    {
        public string Message { get; init; }
        public Point Position { get; init; } = new Point(0.5, 0.5);
    }
}
