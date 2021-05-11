using ChessGame.Helpers;
using ChessGame.Model.Figures.Helpers;
using ChessGame.ViewModel;
using ChessGame.Model.Helpers;
using System.Timers;
using System;

namespace ChessGame.Model
{
    class Game : NotifyPropertyChanged
    {
        private TimeSpan _white_remaining_seconds;
        private TimeSpan _black_remaining_seconds;
        private Board _board;
        private Timer _one_second_timer = new Timer(1000);
        public delegate void GameOverHandler(object sender, GameOverEventArgs e);
        public event GameOverHandler GameOver;
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

        public Board Board
        {
            get => _board;
            private set => _board = value;
        }

        public Game(ChessViewModel view_model, int white_total_seconds, int black_total_seconds)
        {
            WhiteRemainingSeconds = new TimeSpan(0,0, white_total_seconds);
            BlackRemainingSeconds = new TimeSpan(0, 0, black_total_seconds);
            Board = new Board(view_model);
            _one_second_timer.Elapsed += Timer_Elapsed;
            _one_second_timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if(_board.CurrentMoveColor == FigureColor.White)
            {
                WhiteRemainingSeconds = WhiteRemainingSeconds.Subtract(new TimeSpan(0, 0, 1));
                if (WhiteRemainingSeconds.TotalSeconds <= 0)
                {
                    _one_second_timer.Stop();
                    GameOver?.Invoke(this, new GameOverEventArgs(FigureColor.Black));
                }
            }
            else
            {
                BlackRemainingSeconds = BlackRemainingSeconds.Subtract(new TimeSpan(0, 0, 1));
                if (BlackRemainingSeconds.TotalSeconds <= 0)
                {
                    _one_second_timer.Stop();
                    GameOver?.Invoke(this, new GameOverEventArgs(FigureColor.White));
                }
            }
        }
    }
}
