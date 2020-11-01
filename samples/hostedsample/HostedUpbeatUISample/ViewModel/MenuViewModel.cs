/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System.Windows.Input;
using UpbeatUI.Extensions.Hosting;
using UpbeatUI.ViewModel;

namespace UpbeatUISample.ViewModel
{
    public class MenuViewModel
    {
        public MenuViewModel(IUpbeatApplicationService hostedUpbeatService) =>
            ExitCommand = new DelegateCommand(hostedUpbeatService.CloseUpbeatApplication);

        public ICommand ExitCommand { get; }

        public class Parameters
        { }
    }
}
