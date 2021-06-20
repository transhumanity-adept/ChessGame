using System;
using System.Windows.Input;

namespace ChessGame.ViewModel
{
    /// <summary>
    /// Класс представляющий команду
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region Конструкторы
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        #endregion

        #region Делегаты
        /// <summary>
        /// Тело команды
        /// </summary>
        private readonly Action<object> _execute;
        /// <summary>
        /// Условие выполнения команды
        /// </summary>
        private readonly Func<object, bool> _canExecute;
        #endregion

        #region События
        /// <summary>
        /// Событие "Изменился источник команды"
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        #endregion

        #region Методы
        /// <summary>
        /// Выполнение условий вызова команды
        /// </summary>
        /// <param name="parameter">Параметр команды</param>
        /// <returns>Возможность выполнения</returns>
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);
        /// <summary>
        /// Выполнение команды
        /// </summary>
        /// <param name="parameter">Параметр команды</param>
        public void Execute(object parameter) => _execute(parameter);
        #endregion
    }
}
