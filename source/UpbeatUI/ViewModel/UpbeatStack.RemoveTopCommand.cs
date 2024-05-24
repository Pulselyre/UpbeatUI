/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Linq;
using System.Windows.Input;

namespace UpbeatUI.ViewModel
{
    public partial class UpbeatStack
    {
        private sealed class RemoveTopCommand : ICommand
        {
            private readonly UpbeatStack _upbeatStack;

            public RemoveTopCommand(UpbeatStack upbeatStack) =>
                _upbeatStack = upbeatStack ?? throw new ArgumentNullException(nameof(upbeatStack));

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter) =>
                _upbeatStack.CanRemoveTopViewModel();

            public async void Execute(object parameter)
            {
                if (CanExecute(parameter))
                {
                    _ = await _upbeatStack.TryRemoveViewModelAsync(_upbeatStack._openViewModels.Last()).ConfigureAwait(true);
                }
            }

            public void NotifyCanExecuteChanged() =>
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
