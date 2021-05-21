using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatClientCS.Commands
{
    public class RelayCommandAsync : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Predicate<object> _canExecute;
        private readonly Action<Exception> _onException;
        private bool isExecuting;

        public RelayCommandAsync(Func<Task> execute) : this(execute, null,null) { }

        public RelayCommandAsync(Func<Task> execute, Predicate<object> canExecute, Action<Exception> onException)
        {
            _execute = execute;
            _canExecute = canExecute;
            _onException = onException;
        }

        public bool CanExecute(object parameter)
        {
            if (!isExecuting && _canExecute == null) return true;
            return (!isExecuting && _canExecute(parameter));
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public async void Execute(object parameter)
        {
            isExecuting = true;
            try { await _execute(); }
            catch (Exception ex){ _onException?.Invoke(ex); }
            finally { isExecuting = false; }
        }
    }
}
