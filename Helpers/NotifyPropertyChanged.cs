using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ChessGame.Helpers
{
    /// <summary>
    /// Класс, сообщающий об изменении свойств
    /// </summary>
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        #region События
        /// <summary>
        /// Событие "Свойство изменилось"
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        
        #region Методы
        /// <summary>
        /// Метод вызывающий событие "Свойство изменилось"
        /// </summary>
        /// <param name="propertyName">Название свойства</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
