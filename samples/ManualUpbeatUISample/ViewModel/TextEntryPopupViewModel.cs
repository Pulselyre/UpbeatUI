/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using CommunityToolkit.Mvvm.Input;
using UpbeatUI.ViewModel;

namespace ManualUpbeatUISample.ViewModel;

internal sealed partial class TextEntryPopupViewModel : PopupViewModel
{
    private readonly IUpbeatService _upbeatService;
    private readonly Parameters _parameters;

    public TextEntryPopupViewModel
        (IUpbeatService upbeatService, // This will be a unique IUpbeatService created and injected by the IUpbeatStack specifically for this ViewModel.
        Parameters parameters, // These are the parameters the parent used when opening this ViewModel. The IUpbeatService can inject the Parameters object into this constructor to pass initialization data or callbacks.
        SharedTimer sharedTimer) // This is a shared singleton service.
        : base(parameters, sharedTimer)
    {
        _upbeatService = upbeatService ?? throw new ArgumentNullException(nameof(upbeatService));
        _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
    }

    // RelayCommand is an ICommand implementation from the CommunityToolkit.Mvvm NuGet package. As an attribute, it can be used to automatically wrap methods within ICommand properties. It supports both async and non-async methods/lambdas.
    [RelayCommand]
    private void Return(string entryString)
    {
        _parameters?.ReturnCallback?.Invoke(entryString);
        // Will close this ViewModel.
        _upbeatService.Close();
    }

    // This nested Parameters class (full class name: "ConfirmPopupViewModel.Parameters") is what other ViewModels will create instances of to tell the IUpbeatStack what type of child ViewModel to add to the stack.
    public sealed new class Parameters : PopupViewModel.Parameters
    {
        public Action<string> ReturnCallback { get; init; }
    }
}
