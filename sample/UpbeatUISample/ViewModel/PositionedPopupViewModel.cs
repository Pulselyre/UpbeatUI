/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System.Windows;
using UpbeatUI.ViewModel;

namespace UpbeatUISample.ViewModel
{
    internal class PositionedPopupViewModel : PopupViewModel
    {
        public PositionedPopupViewModel(IUpbeatService upbeatService, string message, Point point)
            : base(upbeatService, message) =>
            PositionViewModel = new PositionViewModel(point);

        public PositionViewModel PositionViewModel { get; }
    }
}
