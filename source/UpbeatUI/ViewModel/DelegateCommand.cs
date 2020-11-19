/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UpbeatUI.ViewModel
{
    /// <summary>
    /// Provides a convenient means of creating an <see cref="ICommand"/> using delegates provided by the parent class.
    /// </summary>
    public sealed class DelegateCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;
        private bool _isAsyncExecuting = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class that can always invoke the <paramref name="execute"/> delegate.
        /// </summary>
        /// <param name="execute">The delegate to be invoked when the command is executed.</param>
        public DelegateCommand(Action execute)
            : this(execute, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class that can invoke the <paramref name="execute"/> delegate when the <paramref name="canExecute"/> delegate returns true;
        /// </summary>
        /// <param name="execute">The delegate to be invoked when the command is executed.</param>
        /// <param name="canExecute">The delegate to test if the command can be executed.</param>
        public DelegateCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Initializes a new asynchronous instance of the <see cref="DelegateCommand"/> class that can always invoke the <paramref name="executeAsync"/> delegate.
        /// </summary>
        /// <param name="executeAsync">The delegate to be invoked when the command is executed.</param>
        /// <param name="errorCallback">The delegate that will be executed if the <paramref name="executeAsync"/> delegate fails.</param>
        public DelegateCommand(Func<Task> executeAsync, Action<Exception> errorCallback = null)
            : this(executeAsync, null, errorCallback) { }

        /// <summary>
        /// Initializes a new asynchronous instance of the <see cref="DelegateCommand"/> class that can invoke the <paramref name="executeAsync"/> delegate when the <paramref name="canExecute"/> delegate returns true;
        /// </summary>
        /// <param name="executeAsync">The delegate to be invoked when the command is executed.</param>
        /// <param name="canExecute">The delegate to test if the command can be executed.</param>
        /// <param name="errorCallback">The delegate that will be executed if the <paramref name="executeAsync"/> delegate fails.</param>
        /// <param name="singleExecution">If true, the command will allow only one async operation to execute at once.</param>
        public DelegateCommand(Func<Task> executeAsync, Func<bool> canExecute, Action<Exception> errorCallback = null, bool singleExecution = true)
        {
            if (executeAsync == null)
                throw new ArgumentNullException(nameof(executeAsync));
            _execute = async () =>
            {
                if (!CanExecute(null))
                    return;
                try
                {
                    _isAsyncExecuting = singleExecution;
                    await executeAsync();
                }
                catch (Exception e)
                {
                    errorCallback?.Invoke(e);
                }
                finally
                {
                    _isAsyncExecuting = false;
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
            !_isAsyncExecuting && (_canExecute?.Invoke() ?? true);

        public void Execute(object parameter) =>
            _execute.Invoke();
    }

    /// <summary>
    /// Provides a convenient means of creating an <see cref="ICommand"/> with a parameter using delegates provided by the parent class.
    /// </summary>
    /// <typeparam name="T">The type of the command's parameter.</typeparam>
    public sealed class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;
        private bool _isAsyncExecuting = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class that can always invoke the <paramref name="execute"/> delegate.
        /// </summary>
        /// <param name="execute">The delegate to be invoked when the command is executed.</param>
        public DelegateCommand(Action<T> execute)
            : this(execute, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class that can invoke the <paramref name="execute"/> delegate when the <paramref name="canExecute"/> delegate returns true;
        /// </summary>
        /// <param name="execute">The delegate to be invoked when the command is executed.</param>
        /// <param name="canExecute">The delegate to test if the command can be executed.</param>
        public DelegateCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Initializes a new asynchronous instance of the <see cref="DelegateCommand"/> class that can always invoke the <paramref name="executeAsync"/> delegate.
        /// </summary>
        /// <param name="executeAsync">The delegate to be invoked when the command is executed.</param>
        /// <param name="errorCallback">The delegate that will be executed if the <paramref name="executeAsync"/> delegate fails.</param>
        public DelegateCommand(Func<T, Task> executeAsync, Action<Exception> errorCallback = null)
            : this(executeAsync, null, errorCallback) { }

        /// <summary>
        /// Initializes a new asynchronous instance of the <see cref="DelegateCommand"/> class that can invoke the <paramref name="executeAsync"/> delegate when the <paramref name="canExecute"/> delegate returns true;
        /// </summary>
        /// <param name="executeAsync">The delegate to be invoked when the command is executed.</param>
        /// <param name="canExecute">The delegate to test if the command can be executed.</param>
        /// <param name="errorCallback">The delegate that will be executed if the <paramref name="executeAsync"/> delegate fails.</param>
        /// <param name="singleExecution">If true, the command will allow only one async operation to execute at once.</param>
        public DelegateCommand(Func<T, Task> executeAsync, Predicate<T> canExecute, Action<Exception> errorCallback = null, bool singleExecution = true)
        {
            if (executeAsync == null)
                throw new ArgumentNullException(nameof(executeAsync));
            _execute = async commandParameter =>
            {
                if (!CanExecute(commandParameter))
                    return;
                try
                {
                    _isAsyncExecuting = singleExecution;
                    await executeAsync(commandParameter);
                }
                catch (Exception e)
                {
                    errorCallback?.Invoke(e);
                }
                finally
                {
                    _isAsyncExecuting = false;
                }
            };
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { if (_canExecute != null) CommandManager.RequerySuggested += value; }
            remove { if (_canExecute != null) CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) =>
            !_isAsyncExecuting &&
            (_canExecute?.Invoke((parameter == null && typeof(T).IsValueType) ? default : (T)parameter) ?? true);

        public void Execute(object parameter) =>
            _execute((parameter == null && typeof(T).IsValueType) ? default : (T)parameter);
    }
}
