using System;
using System.Windows.Input;

namespace ConstructionERP_DesktopUI.Helpers
{
    public class RelayCommand : ICommand
    {
        protected readonly Func<bool> canExecute;
        protected readonly Action<object> execute;

        public RelayCommand(Action<object> execute)
           : this(execute, null)
        {
        }

        public RelayCommand(Action<object> execute, Func<bool> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (this.canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }

            remove
            {
                if (this.canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        public virtual Boolean CanExecute(Object parameter)
        {
            return this.canExecute == null ? true : this.canExecute();
        }

        public virtual void Execute(Object parameter)
        {
            this.execute((object)parameter);
        }
    }
}
