using System;
using System.Windows.Input;

namespace XBindLecture.ViewModels
{
	public sealed class RelayCommand : ICommand
	{
		private readonly Action _execute;
		private readonly Func<bool> _canExecute;

		public RelayCommand(Action execute, Func<bool> canExecute)
		{
			if (execute == null) throw new ArgumentNullException(nameof(execute));
			if (canExecute == null) throw new ArgumentNullException(nameof(canExecute));

			this._execute = execute;
			this._canExecute = canExecute;
		}

		public void Execute(object parameter) => this._execute();
		public bool CanExecute(object parameter) => this._canExecute();

		public void RaiseCanExecuteChanged()
			=> this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		public event EventHandler CanExecuteChanged;
	}
}