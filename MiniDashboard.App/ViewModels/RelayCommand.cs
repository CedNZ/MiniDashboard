using MiniDashboard.App.Commands;

namespace MiniDashboard.App.ViewModels
{
    public class RelayCommand : CommandBase
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        protected override bool CanExecute(object? parameter) =>
            _canExecute?.Invoke() ?? true;

        protected override void Execute(object? parameter) =>
            _execute();
    }

    public class RelayCommand<T> : CommandBase
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool>? _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        protected override bool CanExecute(object? parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            if (parameter == null)
            {
                // Allow null when T is a reference or nullable type
                if (!typeof(T).IsValueType || Nullable.GetUnderlyingType(typeof(T)) != null)
                {
                    return _canExecute((T)(object?)parameter!);
                }

                return false; // value type + null -> cannot execute
            }

            if (parameter is T value)
            {
                return _canExecute(value);
            }

            // wrong type passed in – be conservative
            return false;
        }

        protected override void Execute(object? parameter)
        {
            if (parameter == null)
            {
                if (!typeof(T).IsValueType || Nullable.GetUnderlyingType(typeof(T)) != null)
                {
                    _execute((T)(object?)parameter!);
                }

                return; // value type + null: do nothing
            }

            if (parameter is T value)
            {
                _execute(value);
            }
        }
    }
}
