/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Windows.Input;

namespace UpbeatUI.ViewModel
{
    /// <summary>
    /// Provides a convenient means of creating an ICommand using delegates provided by the parent class.
    /// </summary>
    public sealed class ObservableCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        /// <summary>
        /// Initializes a new instance of the ObservableCommand class that can always invoke the execute method.
        /// </summary>
        /// <param name="execute">The method to be invoked when the command is executed.</param>
        public ObservableCommand(Action execute)
            : this(execute, null) { }

        /// <summary>
        /// Initializes a new instance of the ObservableCommand class that can invoke the execute method when the canExecute method returns true;
        /// </summary>
        /// <param name="execute">The method to be invoked when the command is executed.</param>
        /// <param name="canExecute">The method to test if the command can be executed.</param>
        public ObservableCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) =>
            _canExecute?.Invoke() ?? true;

        public void Execute(object parameter) =>
            _execute?.Invoke();
    }

    public sealed class ObservableCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        /// <summary>
        /// Initializes a new instance of the ObservableCommand class that can always invoke the execute method.
        /// </summary>
        /// <param name="execute">The method to be invoked when the command is executed.</param>
        public ObservableCommand(Action<T> execute)
            : this(execute, null) { }

        /// <summary>
        /// Initializes a new instance of the ObservableCommand class that can invoke the execute method when the canExecute method returns true;
        /// </summary>
        /// <param name="execute">The method to be invoked when the command is executed.</param>
        /// <param name="canExecute">The method to test if the command can be executed.</param>
        public ObservableCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { if (_canExecute != null) CommandManager.RequerySuggested += value; }
            remove { if (_canExecute != null) CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if (parameter == null && typeof(T).IsValueType)
                return CanExecute(default(T));
            return _canExecute == null ? true : _canExecute((T)parameter);
        }

        public void Execute(object parameter) =>
            _execute((T)parameter);
    }
}
