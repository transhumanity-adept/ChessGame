using ChessGame.Helpers;

namespace ChessGame.Model
{
    /// <summary>
    /// Игрок
    /// </summary>
    public class Player : NotifyPropertyChanged
    {
        #region Поля
        private int _rating;
        #endregion

        #region Контрукторы
        public Player(int rating, string login) => (Rating, Login) = (rating, login);
        #endregion

        #region Свойства
        public int Rating
        {
            get => _rating;
            set
            {
                _rating = value;
                OnPropertyChanged();
            }
        }
        public string Login { get; }
        #endregion
    }
}
