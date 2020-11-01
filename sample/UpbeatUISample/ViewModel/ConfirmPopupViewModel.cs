/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Windows.Input;
using UpbeatUI.ViewModel;

namespace UpbeatUISample.ViewModel
{
    internal class ConfirmPopupViewModel : PopupViewModel
    {
        public ConfirmPopupViewModel(IUpbeatService upbeatService, Parameters parameters)
            : base(upbeatService, parameters) =>
            ConfirmCommand = new DelegateCommand(
                () =>
                {
                    parameters.Confirm();
                    upbeatService.Close();
                });

        public ICommand ConfirmCommand { get; }

        public new class Parameters : PopupViewModel.Parameters
        {
            public Parameters(string message, Action confirm)
                : base(message) =>
                Confirm = confirm;

            public Action Confirm { get; }
        }
    }
}
