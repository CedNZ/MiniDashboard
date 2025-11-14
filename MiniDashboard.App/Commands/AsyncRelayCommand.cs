namespace MiniDashboard.App.Commands
{
    public class AsyncRelayCommand : CommandBase
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool>? _canExecute;
        private bool _isRunning;

        public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute)); 
            _canExecute = canExecute;
        }

        public override bool CanExecute(object? parameter) =>
            !_isRunning && (_canExecute?.Invoke() ?? true);

        public override async void Execute(object? parameter)
        {
            try
            {
                _isRunning = true;
                RaiseCanExecuteChanged();
                await _execute();
            }
            finally
            {
                _isRunning = false;
                RaiseCanExecuteChanged();
            }
        }
    }

    public class AsyncRelayCommand<T> : CommandBase
    {
        private readonly Func<T, Task> _execute;
        private readonly Func<T, bool>? _canExecute;
        private bool _isExecuting;

        public AsyncRelayCommand(Func<T, Task> execute, Func<T, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public override bool CanExecute(object? parameter)
        {
            if (_isExecuting)
            {
                return false;
            }

            if (_canExecute == null)
            {
                return true;
            }

            if (parameter == null)
            {
                // allow null for reference or nullable types
                if (!typeof(T).IsValueType || Nullable.GetUnderlyingType(typeof(T)) != null)
                {
                    return _canExecute((T)(object?)parameter!);
                }

                return false; // null passed to non-nullable value type
            }

            return parameter is T value && _canExecute(value);
        }

        public override async void Execute(object? parameter)
        {
            if (!CanExecute(parameter))
            {
                return;
            }

            try
            {
                _isExecuting = true;
                RaiseCanExecuteChanged();

                if (parameter == null)
                {
                    // null allowed for reference or nullable types
                    if (!typeof(T).IsValueType || Nullable.GetUnderlyingType(typeof(T)) != null)
                    {
                        await _execute((T)(object?)parameter!);
                        return;
                    }

                    return; // null+value type => shouldn't happen
                }

                if (parameter is T value)
                {
                    await _execute(value);
                }
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }
    }
}
