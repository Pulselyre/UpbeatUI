/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;

namespace HostedUpbeatUISample;

// This is a simple list wrapper to demonstrate a scoped service shared between multiple ViewModels.
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
