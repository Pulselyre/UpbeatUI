/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UpbeatUI.ViewModel;

namespace UpbeatUISample.ViewModel
{
    public class BottomViewModel
    {
        private IUpbeatService _upbeatService;

        public BottomViewModel(IUpbeatService upbeatService)
        {
            _upbeatService = upbeatService;
            _upbeatService.RegisterCloseCallback(AskBeforeClosingAsync);

            OpenCenterPopupCommand = new DelegateCommand(
                () => _upbeatService.OpenViewModel(
                    new PopupViewModel.Parameters("This popup appears in the center of the screen.")));
            OpenMenuCommand = new DelegateCommand(
                () => _upbeatService.OpenViewModel(
                    new MenuViewModel.Parameters()));
            OpenPositionedPopupCommand = new DelegateCommand<Func<Point>>(
                pointGetter => _upbeatService.OpenViewModel(
                    new PositionedPopupViewModel.Parameters(
                        "This popup appears on top of\nthe button that opened it.",
                        pointGetter())));
            OpenSizedPopupCommand = new DelegateCommand(
                () => _upbeatService.OpenViewModel(
                    new ScaledPopupViewModel.Parameters("This popup automatically scales to the window size.\nTry resizing the window to see.")));
        }

        public ICommand OpenCenterPopupCommand { get; }
        public ICommand OpenMenuCommand { get; }
        public ICommand OpenPositionedPopupCommand { get; }
        public ICommand OpenSizedPopupCommand { get; }

        private async Task<bool> AskBeforeClosingAsync()
        {
            bool okToClose = false;
            await _upbeatService.OpenViewModelAsync(
                new ConfirmPopupViewModel.Parameters(
                    "The application is trying to exit.\nClick Confirm to exit or off this popup to cancel.",
                    () => okToClose = true));
            return okToClose;
        }

        public class Parameters
        { }
    }
}
