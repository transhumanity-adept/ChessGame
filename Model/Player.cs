using ChessGame.Helpers;
using ChessGame.Model.Figures.Helpers;
using ChessGame.Model.Helpers;
using ChessGame.ViewModel;

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
        public Player(Game game, int rating, string login, FigureColor side_color)
        {
            (Rating, Login, SideColor) = (rating, login, side_color);
            game.RatingChanged += ChessGameRatingChanged;
        }
        #endregion

        #region Свойства
        public int Rating
        {
            get => _rating;
            private set
            {
                _rating = value;
                OnPropertyChanged();
            }
        }
        public string Login { get; }
        public FigureColor SideColor { get; }
        #endregion

        #region Методы
        /// <summary>
        /// Обработчик события "Рейтинг изменился"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="game_result">Результат игры</param>
        private void ChessGameRatingChanged(object sender, GameResult game_result)
        {
            if (game_result == GameResult.WhiteWin)
                Rating = SideColor == FigureColor.White ? Rating + 25 : Rating - 25;
            if (game_result == GameResult.BlackWin)
                Rating = SideColor == FigureColor.White ? Rating - 25 : Rating + 25;
        }
        #endregion
    }
}
