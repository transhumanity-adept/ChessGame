using ChessGame.Helpers;
using ChessGame.Model.Figures.Helpers;
using ChessGame.ViewModel;
using ChessGame.Model.Helpers;
using System.Timers;
using System;

namespace ChessGame.Model
{
    public class Game : NotifyPropertyChanged
    {
        private TimeSpan _white_remaining_seconds;
        private TimeSpan _black_remaining_seconds;
        private Board _board;
        private Timer _one_second_timer = new Timer(1000);
        private ChessViewModel _chess_vm;
        public delegate void GameOverHandler(object sender, GameOverEventArgs e);
        public event GameOverHandler GameOver;
        public event Action EventsDetached;
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

        public Game(DateTime save_date, ChessViewModel view_model, int white_total_seconds, int black_total_seconds)
        {
            _chess_vm = view_model;
            SaveDate = save_date;
            WhiteRemainingSeconds = new TimeSpan(0,0, white_total_seconds);
            BlackRemainingSeconds = new TimeSpan(0, 0, black_total_seconds);
            Board board = new Board(this, view_model);
            board.GameOver += BoardGameOver;
            board.EventsDetached += BoardEventsDetached;
            Board = board;
            _one_second_timer.Elapsed += Timer_Elapsed;
            _one_second_timer.Start();
        }

        private void BoardEventsDetached()
        {
            _one_second_timer.Stop();
            EventsDetached?.Invoke();
        }

        private void BoardGameOver(object sender, GameOverEventArgs e)
        {
            _one_second_timer.Stop();
            GameOver?.Invoke(this, e);
        }

        public Game(DateTime save_date, ChessViewModel view_model, string restore_state_info)
        {
            _chess_vm = view_model;
            SaveDate = save_date;
            RestoreState(view_model, restore_state_info);
            _one_second_timer.Elapsed += Timer_Elapsed;
            _one_second_timer.Start();
        }

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

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if(_board.CurrentMoveColor == FigureColor.White)
            {
                WhiteRemainingSeconds = WhiteRemainingSeconds.Subtract(new TimeSpan(0, 0, 1));
                if (WhiteRemainingSeconds.TotalSeconds <= 0)
                {
                    _one_second_timer.Stop();
                    GameOver?.Invoke(this, new GameOverEventArgs(GameResult.BlackWin));
                }
            }
            else
            {
                BlackRemainingSeconds = BlackRemainingSeconds.Subtract(new TimeSpan(0, 0, 1));
                if (BlackRemainingSeconds.TotalSeconds <= 0)
                {
                    _one_second_timer.Stop();
                    GameOver?.Invoke(this, new GameOverEventArgs(GameResult.WhiteWin));
                }
            }
        }

        public override string ToString()
        {
            return $"{WhiteRemainingSeconds}-{BlackRemainingSeconds}-{_board}";
        }
    }
}
