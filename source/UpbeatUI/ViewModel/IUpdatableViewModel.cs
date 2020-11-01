/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;

namespace UpbeatUI.ViewModel
{
    /// <summary>
    /// Defines functionality for ViewModels that need to be updated for each frame render.
    /// </summary>
    [Obsolete("The IUpbeatService.SetUpdateCallback is the preffered mechanism for configuring a ViewModel to update on each frame render, and this interface will be removed in UpbeatUI 3.0")]
    public interface IUpdatableViewModel
    {
        /// <summary>
        /// Signals that a new frame is being rendered so that the ViewModel can update its properties.
        /// </summary>
        [Obsolete("The IUpbeatService.SetUpdateCallback is the preffered mechanism for configuring a ViewModel to update on each frame render, and this interface will be removed in UpbeatUI 3.0")]
        void UpdateViewModelProperties();
    }
}
