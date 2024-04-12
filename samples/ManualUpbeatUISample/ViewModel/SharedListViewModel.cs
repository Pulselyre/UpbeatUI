/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UpbeatUI.ViewModel;

namespace ManualUpbeatUISample.ViewModel;

// This extends ObservableObject from the CommunityToolkit.Mvvm NuGet package, which provides pre-written SetProperty and OnPropertyChanged methods.
public sealed partial class SharedListViewModel : ObservableObject, IDisposable
{
    private readonly IUpbeatService _upbeatService;
    private readonly SharedTimer _sharedTimer;
    private readonly SharedList _sharedList;

    public SharedListViewModel(
        IUpbeatService upbeatService, // This will be a unique IUpbeatService created and injected by the IUpbeatStack for this ViewModel and child ViewModels.
        SharedList sharedList, // This is a scoped service shared between this ViewModel and other ViewModel or scoped/transient service dependencies.
        SharedTimer sharedTimer, // This is a shared singleton service.
        SharedListDataViewModel sharedListDataViewModel) // This is a child ViewModel, which can help with separating concerns and keep ViewModels from being too complicated. Child ViewModels share an IUpbeatService and any scoped services with their parents.
    {
        _upbeatService = upbeatService ?? throw new ArgumentNullException(nameof(upbeatService));
        _sharedTimer = sharedTimer ?? throw new ArgumentNullException(nameof(sharedTimer));
        _sharedList = sharedList ?? throw new ArgumentNullException(nameof(sharedList));
        SharedListDataViewModel = sharedListDataViewModel ?? throw new ArgumentNullException(nameof(sharedListDataViewModel));

        _sharedTimer.Ticked += SharedTimerTicked;
        _sharedList.StringAdded += SharedListStringAdded;
    }

    public string StringsCount => $"{_sharedList.Strings.Count} Strings";
    public string SecondsElapsed => $"{_sharedTimer.ElapsedSeconds} Seconds";
    public SharedListDataViewModel SharedListDataViewModel { get; }

    // RelayCommand is an ICommand implementation from the CommunityToolkit.Mvvm NuGet package. As an attribute, it can be used to automatically wrap methods within ICommand properties. It supports both async and non-async methods/lambdas.
    [RelayCommand]
    private void Close() => _upbeatService.Close();

    public void Dispose() =>
        _sharedTimer.Ticked -= SharedTimerTicked;

    private void SharedListStringAdded(object sender, EventArgs e) =>
        Application.Current.Dispatcher.Invoke(() => OnPropertyChanged(nameof(StringsCount))); // Ensure that the PropertyChanged event is raised on the UI thread

    private void SharedTimerTicked(object sender, EventArgs e) =>
        Application.Current.Dispatcher.Invoke(() => OnPropertyChanged(nameof(SecondsElapsed))); // Ensure that the PropertyChanged event is raised on the UI thread

    // This nested Parameters class (full class name: "BottomViewModel.Parameters") is what other ViewModels will create instances of to tell the IUpbeatStack what type of child ViewModel to add to the stack.
    public class Parameters
    { }
}
