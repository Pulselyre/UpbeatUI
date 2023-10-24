/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UpbeatUI.ViewModel;

namespace ServiceProvidedUpbeatUISample.ViewModel;

// This extends ObservableObject from the CommunityToolkit.Mvvm NuGet package, which provides pre-written SetProperty and OnPropertyChanged methods.
public class BottomViewModel : ObservableObject, IDisposable
{
    private readonly IUpbeatService _upbeatService;
    private readonly SharedTimer _sharedTimer;

    public BottomViewModel(
        IUpbeatService upbeatService, // This will be a unique IUpbeatService created and injected by the IUpbeatStack specifically for this ViewModel.
        SharedTimer sharedTimer) // This is a shared singleton service.
    {
        _upbeatService = upbeatService ?? throw new NullReferenceException(nameof(upbeatService));
        _sharedTimer = sharedTimer ?? throw new NullReferenceException(nameof(sharedTimer));

        // Registering a CloseCallback allows the ViewModel to prevent itself from closing. For example: if there is unsaved work. This can also completely prevent the application from shutting down. CloseCallbacks can be either async or non-async methods/lambdas.
        _upbeatService.RegisterCloseCallback(AskBeforeClosingAsync);

        _sharedTimer.Ticked += SharedTimerTicked;

        // RelayCommand is an ICommand implementation from the CommunityToolkit.Mvvm NuGet package. It can be used to call methods or lambda expressions when the command is executed. It supports both async and non-async methods/lambdas.
        OpenMenuCommand = new RelayCommand(
            () => _upbeatService.OpenViewModel( // Create a Parameters object for a ViewModel and pass it to the IUpbeatStack using OpenViewModel. The IUpbeatStack will use the configured mappings to create the appropriate ViewModel from the Parameters type.
                new MenuViewModel.Parameters()));
        OpenSharedListCommand = new RelayCommand(
            () => _upbeatService.OpenViewModel(
                new SharedListViewModel.Parameters()));
        OpenRandomDataCommand = new RelayCommand(
            () => _upbeatService.OpenViewModel(
                new RandomDataViewModel.Parameters()));
    }

    public ICommand OpenMenuCommand { get; }
    public ICommand OpenSharedListCommand { get; }
    public ICommand OpenRandomDataCommand { get; }
    public string SecondsElapsed => $"{_sharedTimer.ElapsedSeconds} Seconds";

    public void Dispose() =>
        _sharedTimer.Ticked -= SharedTimerTicked;

    // This CloseCallback method opens a new ViewModel and View to confirm that the user wants to close this ViewModel.
    private async Task<bool> AskBeforeClosingAsync()
    {
        var okToClose = false;
        // OpenViewModelAsync can be awaited, and will return once the child ViewModel is closed. This is useful to show a popup requesting input from the user.
        await _upbeatService.OpenViewModelAsync(
            new ConfirmPopupViewModel.Parameters
            {
                Message = "The application is trying to exit.\nClick Confirm to exit or off this popup to cancel.",
                // The ConfirmPopupViewModel will execute this callback (set the okToClose bool to true) if the user confirms that closing. If the popup closes without the user confirming, okToClose remains false, and the application will remain running.
                ConfirmCallback = () => okToClose = true,
            });
        return okToClose;
    }

    private void SharedTimerTicked(object sender, EventArgs e) =>
        Application.Current.Dispatcher.Invoke(() => OnPropertyChanged(nameof(SecondsElapsed))); // Ensure that the PropertyChanged event is raised on the UI thread

    // This nested Parameters class (full class name: "BottomViewModel.Parameters") is what other ViewModels will create instances of to tell the IUpbeatStack what type of child ViewModel to add to the stack.
    public class Parameters
    { }
}
