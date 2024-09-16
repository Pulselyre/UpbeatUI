/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Diagnostics;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UpbeatUI.Extensions.Hosting;
using UpbeatUI.ViewModel;

namespace HostedUpbeatUISample.ViewModel;

// This extends ObservableObject from the CommunityToolkit.Mvvm NuGet package, which provides pre-written SetProperty and OnPropertyChanged methods.
public sealed partial class MenuViewModel : ObservableObject, IDisposable
{
    private readonly IUpbeatService _upbeatService;
    private readonly IUpbeatApplicationService _hostedUpbeatService;
    private readonly SharedTimer _sharedTimer;
    private readonly OverlayService _overlayService;
    private readonly Stopwatch _stopwatch = new();

    public MenuViewModel(
        IUpbeatService upbeatService, // This will be a unique IUpbeatService created and injected by the IUpbeatStack specifically for this ViewModel.
        IUpbeatApplicationService hostedUpbeatService, // IUpbeatApplicationService provides a method allowing ViewModels to start application shutdown.
        SharedTimer sharedTimer, // This is a shared singleton service.
        OverlayService overlayService) // This is a shared singleton service.
    {
        _upbeatService = upbeatService ?? throw new ArgumentNullException(nameof(upbeatService));
        _hostedUpbeatService = hostedUpbeatService ?? throw new ArgumentNullException(nameof(hostedUpbeatService));
        _sharedTimer = sharedTimer ?? throw new ArgumentNullException(nameof(sharedTimer));
        _overlayService = overlayService ?? throw new ArgumentNullException(nameof(overlayService));
        _stopwatch.Start();
        _upbeatService.RegisterUpdateCallback(() => OnPropertyChanged(nameof(Visibility))); // Registered "UpdateCallbacks" will be called each time the UI thread renders a new frame.

        _sharedTimer.Ticked += SharedTimerTicked;
    }

    public string SecondsElapsed => $"{_sharedTimer.ElapsedSeconds} Seconds";
    public double Visibility => Math.Abs(1000.0 - _stopwatch.ElapsedMilliseconds % 2000) / 1000.0; // Will be calculated on each "OnPropertyChanged(nameof(Visibility))" and used in the View to control visibility of an ellipse. Cycles between full and no visibility every two seconds.
    public bool ShowOverlay
    {
        get => _overlayService.OverlayVisible;
        set => SetProperty(
            _overlayService.OverlayVisible,
            value,
            _overlayService,
            (os, v) => os.OverlayVisible = v);
    }

    // RelayCommand is an ICommand implementation from the CommunityToolkit.Mvvm NuGet package. As an attribute, it can be used to automatically wrap methods within ICommand properties. It supports both async and non-async methods/lambdas.
    [RelayCommand]
    private void Exit() => _hostedUpbeatService.CloseUpbeatApplication();

    [RelayCommand]
    private void OpenRandomData()
    {
        // Create a Parameters object for a ViewModel and pass it to the IUpbeatStack using OpenViewModel. The IUpbeatStack will use the configured mappings to create the appropriate ViewModel from the Parameters type.
        _upbeatService.OpenViewModel(new RandomDataViewModel.Parameters());
        // Since this is Side Menu, it can close after the requested ViewModel is opened.
        _upbeatService.Close();
    }

    [RelayCommand]
    private void OpenSharedList()
    {
        _upbeatService.OpenViewModel(new SharedListViewModel.Parameters());
        _upbeatService.Close();
    }

    public void Dispose() =>
        _sharedTimer.Ticked -= SharedTimerTicked;

    private void SharedTimerTicked(object sender, EventArgs e) =>
        Application.Current.Dispatcher.Invoke(() => OnPropertyChanged(nameof(SecondsElapsed))); // Ensure that the PropertyChanged event is raised on the UI thread

    // This nested Parameters class (full class name: "MenuViewModel.Parameters") is what other ViewModels will create instances of to tell the IUpbeatStack what type of child ViewModel to add to the stack.
    public class Parameters
    { }
}
