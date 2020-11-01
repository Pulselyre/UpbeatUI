/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using UpbeatUI.ViewModel;

namespace UpbeatUISample.ViewModel
{
    public class MenuViewModel
    {
        public MenuViewModel(IUpbeatService upbeatService, Parameters parameters) =>
            ExitCommand = new DelegateCommand(parameters.ExitCallback);

        public ICommand ExitCommand { get; }

        public class Parameters : BottomViewModel.Parameters
        {
            public Parameters(Func<Task> exitCallback)
                : base(exitCallback)
            { }
        }
    }
}
