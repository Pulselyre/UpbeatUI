/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using UpbeatUI.ViewModel;

namespace ServiceProvidedUpbeatUISample.ViewModel;

internal sealed class TextEntryPopupViewModel : PopupViewModel
{
    public TextEntryPopupViewModel
        (IUpbeatService upbeatService, // This will be a unique IUpbeatService created and injected by the IUpbeatStack specifically for this ViewModel.
        Parameters parameters, // These are the parameters the parent used when opening this ViewModel. The IUpbeatService can inject the Parameters object into this constructor to pass initialization data or callbacks.
        SharedTimer sharedTimer) // This is a shared singleton service.
        : base(parameters, sharedTimer) =>
        // RelayCommand is an ICommand implementation from the CommunityToolkit.Mvvm NuGet package. It can be used to call methods or lambda expressions when the command is executed. It supports both async and non-async methods/lambdas.
        ReturnCommand = new RelayCommand<string>(
            entryString =>
            {
                parameters?.ReturnCallback?.Invoke(entryString);
                // Will close this ViewModel.
                upbeatService.Close();
            });

    public ICommand ReturnCommand { get; }

    // This nested Parameters class (full class name: "ConfirmPopupViewModel.Parameters") is what other ViewModels will create instances of to tell the IUpbeatStack what type of child ViewModel to add to the stack.
    public sealed new class Parameters : PopupViewModel.Parameters
    {
        public Action<string> ReturnCallback { get; init; }
    }
}
