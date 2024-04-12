/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UpbeatUI.ViewModel;

namespace HostedUpbeatUISample.ViewModel;

// This extends ObservableObject from the CommunityToolkit.Mvvm NuGet package, which provides pre-written SetProperty and OnPropertyChanged methods.
public sealed partial class BottomViewModel : ObservableObject, IDisposable
{
    private readonly IUpbeatService _upbeatService;
    private readonly SharedTimer _sharedTimer;

    public BottomViewModel(
        IUpbeatService upbeatService, // This will be a unique IUpbeatService created and injected by the IUpbeatStack specifically for this ViewModel.
        SharedTimer sharedTimer) // This is a shared singleton service.
    {
        _upbeatService = upbeatService ?? throw new ArgumentNullException(nameof(upbeatService));
        _sharedTimer = sharedTimer ?? throw new ArgumentNullException(nameof(sharedTimer));

        // Registering a CloseCallback allows the ViewModel to prevent itself from closing. For example: if there is unsaved work. This can also completely prevent the application from shutting down. CloseCallbacks can be either async or non-async methods/lambdas.
        _upbeatService.RegisterCloseCallback(AskBeforeClosingAsync);

        _sharedTimer.Ticked += SharedTimerTicked;
    }

    public string SecondsElapsed => $"{_sharedTimer.ElapsedSeconds} Seconds";

    // RelayCommand is an ICommand implementation from the CommunityToolkit.Mvvm NuGet package. As an attribute, it can be used to automatically wrap methods within ICommand properties. It supports both async and non-async methods/lambdas.
    [RelayCommand]
    private void OpenMenu() =>
        _upbeatService.OpenViewModel(new MenuViewModel.Parameters()); // Create a Parameters object for a ViewModel and pass it to the IUpbeatStack using OpenViewModel. The IUpbeatStack will use the configured mappings to create the appropriate ViewModel from the Parameters type.

    [RelayCommand]
    private void OpenSharedList() =>
        _upbeatService.OpenViewModel(new SharedListViewModel.Parameters());

    [RelayCommand]
    private void OpenRandomData() =>
        _upbeatService.OpenViewModel(new RandomDataViewModel.Parameters());

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
            }).ConfigureAwait(true);
        return okToClose;
    }

    private void SharedTimerTicked(object sender, EventArgs e) =>
        Application.Current.Dispatcher.Invoke(() => OnPropertyChanged(nameof(SecondsElapsed))); // Ensure that the PropertyChanged event is raised on the UI thread

    // This nested Parameters class (full class name: "BottomViewModel.Parameters") is what other ViewModels will create instances of to tell the IUpbeatStack what type of child ViewModel to add to the stack.
    public class Parameters
    { }
}
