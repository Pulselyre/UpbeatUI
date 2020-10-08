/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Windows.Input;
using UpbeatUI.ViewModel;

namespace UpbeatUISample.ViewModel
{
    public class MenuViewModel : UpbeatViewModel
    {
        private IUpbeatService _upbeatService;

        public MenuViewModel(IUpbeatService upbeatService, Action exitCallback)
        {
            _upbeatService = upbeatService;
            ExitCommand = new DelegateCommand(() => exitCallback());
        }

        public ICommand ExitCommand { get; }
    }
}
