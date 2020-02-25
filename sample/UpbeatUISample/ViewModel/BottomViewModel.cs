/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Windows;
using System.Windows.Input;
using UpbeatUI.Context;

namespace UpbeatUISample.ViewModel
{
    public class BottomViewModel : ContextObject
    {
        private IContextService _contextService;

        public BottomViewModel(IContextService contextService, Action exitCallback)
        {
            _contextService = contextService;
            OpenCenterPopupCommand = new ObservableCommand(
                () => _contextService.OpenContext(
                    service => new PopupViewModel(service, "This popup appears in the center of the screen.")));
            OpenMenuCommand = new ObservableCommand(
                () => _contextService.OpenContext(service => new MenuViewModel(service, exitCallback)));
            OpenPositionedPopupCommand = new ObservableCommand<Func<Point>>(
                pointGetter => _contextService.OpenContext(
                    service => new PositionedPopupViewModel(service, "This popup appears on top of\nthe button that opened it.", pointGetter())));
            OpenSizedPopupCommand = new ObservableCommand(
                () => _contextService.OpenContext(
                    service => new ScaledPopupViewModel(service, "This popup automatically scales to the window size.\nTry resizing the window to see.")));
        }

        public ICommand OpenCenterPopupCommand { get; }
        public ICommand OpenMenuCommand { get; }
        public ICommand OpenPositionedPopupCommand { get; }
        public ICommand OpenSizedPopupCommand { get; }
    }
}
