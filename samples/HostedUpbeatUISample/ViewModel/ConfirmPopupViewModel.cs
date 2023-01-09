/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Windows.Input;
using UpbeatUI.ViewModel;

namespace HostedUpbeatUISample.ViewModel;

internal class ConfirmPopupViewModel : PopupViewModel
{
    public ConfirmPopupViewModel(
        IUpbeatService upbeatService, // This will be a unique IUpbeatService created and injected by the IUpbeatStack specifically for this ViewModel.
        Parameters parameters, // These are the parameters the parent used when opening this ViewModel. The IUpbeatService can inject the Parameters object into this constructor to pass initialization data or callbacks.
        SharedTimer sharedTimer // This is a shared singleton service.
    ) : base(parameters, sharedTimer)
    {
        // DelegateCommand is a common convenience ICommand implementation to call methods or lambda expressions when the command is executed. It supports both async and non-async methods/lambdas.
        ConfirmCommand = new DelegateCommand(
            () =>
            {
                parameters?.ConfirmCallback?.Invoke();
                // Will close this ViewModel.
                upbeatService.Close();
            });
    }

    public ICommand ConfirmCommand { get; }

    // This nested Parameters class (full class name: "ConfirmPopupViewModel.Parameters") is what other ViewModels will create instances of to tell the IUpbeatStack what type of child ViewModel to add to the stack.
    public new class Parameters : PopupViewModel.Parameters
    {
        public Action ConfirmCallback { get; init; }
    }
}
