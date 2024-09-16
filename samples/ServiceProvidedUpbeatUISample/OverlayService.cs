/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;

namespace ServiceProvidedUpbeatUISample;

// This is a shared service that ViewModels can use to control visibility of the application overlay.
public class OverlayService
{
    private bool _overlayVisible;

    public event EventHandler OverlayToggled;

    public bool OverlayVisible
    {
        get => _overlayVisible;
        set
        {
            if (value != _overlayVisible)
            {
                _overlayVisible = value;
                OverlayToggled?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
