/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace HostedUpbeatUISample.ViewModel;

// This extends ObservableObject from the CommunityToolkit.Mvvm NuGet package, which provides pre-written SetProperty and OnPropertyChanged methods.
public sealed partial class OverlayViewModel : ObservableObject, IDisposable
{
    private readonly OverlayService _overlayService;
    [ObservableProperty]
    private string _message = "Overlay";
    [ObservableProperty]
    private bool _visible;

    public OverlayViewModel(OverlayService overlayService)
    {
        _overlayService = overlayService ?? throw new ArgumentNullException(nameof(overlayService));
        _overlayService.OverlayToggled += HandleOverlayToggled;
    }

    public void Dispose() =>
        _overlayService.OverlayToggled -= HandleOverlayToggled;

    private void HandleOverlayToggled(object sender, EventArgs e) =>
        Visible = _overlayService.OverlayVisible;
}
