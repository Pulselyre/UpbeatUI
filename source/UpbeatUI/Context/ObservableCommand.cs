/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Windows.Input;

namespace UpbeatUI.Context
{
    public sealed class ObservableCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Action<object> _executeWithParameter;
        private readonly Func<bool> _canExecute;
        private readonly Predicate<object> _canExecuteWithParameter;

        public ObservableCommand(Action execute)
            : this(execute, null) { }

        public ObservableCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        public ObservableCommand(Action<object> execute)
            : this(execute, null) { }

        public ObservableCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _executeWithParameter = execute;
            _canExecuteWithParameter = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if (_execute == null)
                return _canExecuteWithParameter == null ? true : _canExecuteWithParameter(parameter);
            else
                return _canExecute == null ? true : _canExecute();
        }

        public void Execute(object parameter)
        {
            if (_execute == null)
                _executeWithParameter(parameter);
            else
                _execute();
        }
    }

    public sealed class ObservableCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        public ObservableCommand(Action<T> execute)
            : this(execute, null) { }

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

        public void Execute(object parameter) => _execute((T)parameter);
    }
}
