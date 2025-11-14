using System.Windows.Input;

namespace MiniDashboard.App.Commands
{
    public abstract class CommandBase : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        bool ICommand.CanExecute(object? parameter) => CanExecute(parameter);

        void ICommand.Execute(object? parameter) => Execute(parameter);

        /// <summary>
        /// Override this to implement your can-execute logic.
        /// </summary>
        public virtual bool CanExecute(object? parameter) => true;

        /// <summary>
        /// Override this to implement your execute logic.
        /// </summary>
        public abstract void Execute(object? parameter);

        public void RaiseCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
