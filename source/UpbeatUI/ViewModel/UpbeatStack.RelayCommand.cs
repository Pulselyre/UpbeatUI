/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UpbeatUI.ViewModel
{
    public partial class UpbeatStack
    {
        private class RelayCommand : ICommand
        {
            private readonly Action _execute;
            private readonly Func<bool> _canExecute;
            private bool _isAsyncExecuting = false;

            public RelayCommand(Func<Task> executeAsync, Func<bool> canExecute = null, Action<Exception> exceptionCallback = null, bool singleExecution = true)
            {
                if (executeAsync == null)
                    throw new ArgumentNullException(nameof(executeAsync));
                _execute = async () =>
                {
                    try
                    {
                        _isAsyncExecuting = singleExecution;
                        await executeAsync();
                    }
                    catch (Exception e)
                    {
                        if (exceptionCallback == null)
                            throw;
                        exceptionCallback(e);
                    }
                    finally
                    {
                        if (_isAsyncExecuting)
                        {
                            _isAsyncExecuting = false;
                            CommandManager.InvalidateRequerySuggested();
                        }
                    }
                };
                _canExecute = canExecute;
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public bool CanExecute(object parameter) =>
                CanExecute();

            public bool CanExecute() =>
                !_isAsyncExecuting && (_canExecute?.Invoke() ?? true);

            public void Execute(object parameter) =>
                Execute();

            public void Execute()
            {
                if (CanExecute())
                    _execute.Invoke();
            }
        }
    }
}
