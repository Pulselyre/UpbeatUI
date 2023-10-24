/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UpbeatUI.ViewModel;

namespace ManualUpbeatUISample.ViewModel;

// This extends BaseViewModel, which provides pre-written SetProperty and RaisePropertyChanged methods.
public class MenuViewModel : BaseViewModel, IDisposable
{
    private readonly IUpbeatService _upbeatService;
    private readonly SharedTimer _sharedTimer;
    private readonly Stopwatch _stopwatch = new();

    public MenuViewModel(
        IUpbeatService upbeatService, // This will be a unique IUpbeatService created and injected by the IUpbeatStack specifically for this ViewModel.
        Func<Task> closeApplicationCallbackAsync, // This ViewModel requires an async delegate to be provided, so it can start closing the application.
        SharedTimer sharedTimer) // This is a shared singleton service.
    {
        _upbeatService = upbeatService ?? throw new NullReferenceException(nameof(upbeatService));
        _ = closeApplicationCallbackAsync ?? throw new NullReferenceException(nameof(closeApplicationCallbackAsync));
        _sharedTimer = sharedTimer ?? throw new NullReferenceException(nameof(sharedTimer));

        _stopwatch.Start();
        _upbeatService.RegisterUpdateCallback(() => RaisePropertyChanged(nameof(Visibility))); // Registered "UpdateCallbacks" will be called each time the UI thread renders a new frame.

        _sharedTimer.Ticked += SharedTimerTicked;

        // DelegateCommand is a common convenience ICommand implementation to call methods or lambda expressions when the command is executed. It supports both async and non-async methods/lambdas.
        ExitCommand = new DelegateCommand(closeApplicationCallbackAsync);
        OpenRandomDataCommand = new DelegateCommand(
            () =>
            {
                // Create a Parameters object for a ViewModel and pass it to the IUpbeatStack using OpenViewModel. The IUpbeatStack will use the configured mappings to create the appropriate ViewModel from the Parameters type.
                _upbeatService.OpenViewModel(new RandomDataViewModel.Parameters());
                // Since this is Side Menu, it can close after the requested ViewModel is opened.
                _upbeatService.Close();
            });
        OpenSharedListCommand = new DelegateCommand(
            () =>
            {
                _upbeatService.OpenViewModel(new SharedListViewModel.Parameters());
                _upbeatService.Close();
            });
    }

    public ICommand ExitCommand { get; }
    public ICommand OpenRandomDataCommand { get; }
    public ICommand OpenSharedListCommand { get; }
    public string SecondsElapsed => $"{_sharedTimer.ElapsedSeconds} Seconds";
    public double Visibility => Math.Abs(1000.0 - _stopwatch.ElapsedMilliseconds % 2000) / 1000.0; // Will be calculated on each "RaisePropertyChanged(nameof(Visibility))" and used in the View to control visibility of an ellipse. Cycles between full and no visibility every two seconds.

    public void Dispose() =>
        _sharedTimer.Ticked -= SharedTimerTicked;

    private void SharedTimerTicked(object sender, EventArgs e) =>
        Application.Current.Dispatcher.Invoke(() => RaisePropertyChanged(nameof(SecondsElapsed))); // Ensure that the PropertyChanged event is raised on the UI thread

    // This nested Parameters class (full class name: "MenuViewModel.Parameters") is what other ViewModels will create instances of to tell the IUpbeatStack what type of child ViewModel to add to the stack.
    public class Parameters
    { }
}
