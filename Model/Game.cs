using ChessGame.Helpers;
using ChessGame.Model.Figures.Helpers;
using ChessGame.ViewModel;
using ChessGame.Model.Helpers;
using System.Timers;
using System;

namespace ChessGame.Model
{
    /// <summary>
    /// Игра шахматы
    /// </summary>
    public class Game : NotifyPropertyChanged
    {
        #region Поля
        private TimeSpan _white_remaining_seconds;
        private TimeSpan _black_remaining_seconds;
        private Board _board;
        private Timer _one_second_timer = new Timer(1000);
        #endregion

        #region Конструкторы
        public Game(DateTime save_date, ChessViewModel view_model, int white_total_seconds, int black_total_seconds)
        {
            SaveDate = save_date;
            WhiteRemainingSeconds = new TimeSpan(0, 0, white_total_seconds);
            BlackRemainingSeconds = new TimeSpan(0, 0, black_total_seconds);
            Board board = new Board(this, view_model);
            board.GameOver += BoardGameOver;
            board.EventsDetached += BoardEventsDetached;
            Board = board;
            _one_second_timer.Elapsed += TimerElapsed;
            _one_second_timer.Start();
        }
        public Game(DateTime save_date, ChessViewModel view_model, string restore_state_info)
        {
            SaveDate = save_date;
            RestoreState(view_model, restore_state_info);
            _one_second_timer.Elapsed += TimerElapsed;
            _one_second_timer.Start();
        }
        #endregion

        #region Свойства
        public TimeSpan WhiteRemainingSeconds
        {
            get => _white_remaining_seconds;
            private set
            {
                _white_remaining_seconds = value;
                OnPropertyChanged();
            }
        }
        public TimeSpan BlackRemainingSeconds
        {
            get => _black_remaining_seconds;
            private set
            {
                _black_remaining_seconds = value;
                OnPropertyChanged();
            }
        }
        public DateTime SaveDate { get; private set; }
        public Board Board
        {
            get => _board;
            private set => _board = value;
        }
        #endregion

        #region События
        public event Action<object, GameResult> GameOver;
        public event Action<object> EventsDetached;
        #endregion

        #region Методы
        /// <summary>
        /// Обработчик события "Подписки на события обнулены"
        /// </summary>
        /// <param name="sender">Источник события</param>
        private void BoardEventsDetached(object sender)
        {
            _one_second_timer.Stop();
            EventsDetached?.Invoke(this);
        }
        /// <summary>
        /// Обработчик события "Игра окончена"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="game_result">Результат игры</param>
        private void BoardGameOver(object sender, GameResult game_result)
        {
            _one_second_timer.Stop();
            GameOver?.Invoke(this, game_result);
        }
        /// <summary>
        /// Восстановление состояния игры
        /// </summary>
        /// <param name="view_model">ViewModel</param>
        /// <param name="restore_state_info">Информация о состоянии игры в строковом виде</param>
        private void RestoreState(ChessViewModel view_model, string restore_state_info)
        {
            string[] game_info = restore_state_info.Split('-');
            TimeSpan white_remaining_seconds = TimeSpan.Parse(game_info[0]);
            TimeSpan black_remaining_seconds = TimeSpan.Parse(game_info[1]);
            Board restored_board = new Board(this, view_model, game_info[2]);
            restored_board.GameOver += BoardGameOver;
            restored_board.EventsDetached += BoardEventsDetached;
            WhiteRemainingSeconds = white_remaining_seconds;
            BlackRemainingSeconds = black_remaining_seconds;
            Board = restored_board;
        }
        /// <summary>
        /// Обработчик события "Тик таймера"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_board.CurrentMoveColor == FigureColor.White)
            {
                WhiteRemainingSeconds = WhiteRemainingSeconds.Subtract(new TimeSpan(0, 0, 1));
                if (WhiteRemainingSeconds.TotalSeconds <= 0)
                {
                    _one_second_timer.Stop();
                    GameOver?.Invoke(this, GameResult.BlackWin);
                }
            }
            else
            {
                BlackRemainingSeconds = BlackRemainingSeconds.Subtract(new TimeSpan(0, 0, 1));
                if (BlackRemainingSeconds.TotalSeconds <= 0)
                {
                    _one_second_timer.Stop();
                    GameOver?.Invoke(this, GameResult.WhiteWin);
                }
            }
        }
        /// <summary>
        /// Информация о игре в виде строки
        /// </summary>
        /// <returns>Строковое представление игры</returns>
        public override string ToString()
        {
            return $"{WhiteRemainingSeconds}-{BlackRemainingSeconds}-{_board}";
        }
        #endregion
    }
}
