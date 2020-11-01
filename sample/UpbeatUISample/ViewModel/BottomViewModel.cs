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
        public BottomViewModel(IUpbeatService upbeatService, Parameters parameters)
        {
            OpenCenterPopupCommand = new DelegateCommand(
                () => upbeatService.OpenViewModel(
                    new PopupViewModel.Parameters("This popup appears in the center of the screen.")));
            OpenMenuCommand = new DelegateCommand(
                () => upbeatService.OpenViewModel(
                    new MenuViewModel.Parameters(parameters.ExitCallback)));
            OpenPositionedPopupCommand = new DelegateCommand<Func<Point>>(
                pointGetter => upbeatService.OpenViewModel(
                    new PositionedPopupViewModel.Parameters(
                        "This popup appears on top of\nthe button that opened it.",
                        pointGetter())));
            OpenSizedPopupCommand = new DelegateCommand(
                () => upbeatService.OpenViewModel(
                    new ScaledPopupViewModel.Parameters("This popup automatically scales to the window size.\nTry resizing the window to see.")));
        }

        public ICommand OpenCenterPopupCommand { get; }
        public ICommand OpenMenuCommand { get; }
        public ICommand OpenPositionedPopupCommand { get; }
        public ICommand OpenSizedPopupCommand { get; }

        public class Parameters
        {
            public Parameters(Func<Task> exitCallback) =>
                ExitCallback = exitCallback ?? throw new ArgumentNullException(nameof(exitCallback));

            public Func<Task> ExitCallback { get; }
        }
    }
}
