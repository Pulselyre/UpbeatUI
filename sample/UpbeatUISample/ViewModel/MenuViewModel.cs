/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Windows.Input;
using UpbeatUI.ViewModel;

namespace UpbeatUISample.ViewModel
{
    public class MenuViewModel : ContextObject
    {
        private IContextService _contextService;

        public MenuViewModel(IContextService contextService, Action exitCallback)
        {
            _contextService = contextService;
            ExitCommand = new ObservableCommand(() => exitCallback());
        }

        public ICommand ExitCommand { get; }
    }
}
