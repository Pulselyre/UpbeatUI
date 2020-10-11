/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Windows;
using System.Windows.Input;
using UpbeatUI.ViewModel;

namespace UpbeatUISample.ViewModel
{
    public class BottomViewModel : UpbeatViewModel
    {
        private IUpbeatService _upbeatService;

        public BottomViewModel(IUpbeatService upbeatService, Action exitCallback)
        {
            _upbeatService = upbeatService;
            OpenCenterPopupCommand = new DelegateCommand(
                () => _upbeatService.OpenUpbeatViewModel(
                    new PopupViewModel.Parameters("This popup appears in the center of the screen.")));
            OpenMenuCommand = new DelegateCommand(
                () => _upbeatService.OpenUpbeatViewModel(
                    new MenuViewModel.Parameters(exitCallback)));
            OpenPositionedPopupCommand = new DelegateCommand<Func<Point>>(
                pointGetter => _upbeatService.OpenUpbeatViewModel(
                    new PositionedPopupViewModel.Parameters(
                        "This popup appears on top of\nthe button that opened it.",
                        pointGetter())));
            OpenSizedPopupCommand = new DelegateCommand(
                () => _upbeatService.OpenUpbeatViewModel(
                    new ScaledPopupViewModel.Parameters("This popup automatically scales to the window size.\nTry resizing the window to see.")));
        }

        public ICommand OpenCenterPopupCommand { get; }
        public ICommand OpenMenuCommand { get; }
        public ICommand OpenPositionedPopupCommand { get; }
        public ICommand OpenSizedPopupCommand { get; }

        public class Parameters
        {
            public Parameters(Action exitCallback) =>
                ExitCallback = exitCallback ?? throw new ArgumentNullException(nameof(exitCallback));

            public Action ExitCallback { get; }
        }
    }
}
