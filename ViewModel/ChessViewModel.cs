using System.Windows.Input;
using ChessGame.Model;
using ChessGame.Helpers;
using ChessGame.Model.Figures.Helpers;
using ChessGame.View;
using ChessGame.Model.Helpers;
using ChessGame.ViewModel.Helpers;
using System.Windows;
using System.Windows.Controls;
using System;

namespace ChessGame.ViewModel
{
    class ChessViewModel : NotifyPropertyChanged
    {
        #region Поля
        private Game _game;
        private ICommand _cell_click_command;
        private Button _current_clicked_button;
        public delegate void ClickedCellHandled(object sender, CellClickedEventArgs e);
        public event ClickedCellHandled ClickedOnCell;
        public delegate void ResultOfPawnChangeObtainedHandler(object sender, ResultOfPawnChangeObtainedEventArgs e);
        public event ResultOfPawnChangeObtainedHandler ResultOfPawnChangeObtained;
        #endregion

        #region Свойства
        public Game Game
        {
            get => _game;
            set
            {
                _game = value;
                OnPropertyChanged();
            }
        }

        public ICommand CellClickCommand => _cell_click_command ?? (_cell_click_command = new RelayCommand(obj =>
        {
            var (button, cell) = obj as Tuple<Button, Cell>;
            _current_clicked_button = button;
            ClickedOnCell?.Invoke(this, new CellClickedEventArgs(cell));
        }));
        #endregion

        #region Конструкторы
        public ChessViewModel()
        {
            Game = new Game(this, 1000, 1000);
            //Window window = new LoginRegistrationWindow();
            //window.ShowDialog();
            //App.Current.MainWindow.Close();
            Game.GameOver += GameOver;
            Game.Board.PawnChanged += Board_PawnChanged;
            Game.Board.GameOver += GameOver;
        }

        private void GameOver(object sender, GameOverEventArgs e)
        {
            Game = null;
            GameResultWindow game_result = new GameResultWindow(e.GameResult, App.Current.MainWindow);
            game_result.ShowDialog();
        }
        #endregion

        /// <summary>
        /// Обработка события "Смена пешки"
        /// </summary>
        private void Board_PawnChanged(object sender, PawnChangedEventArgs e)
        {
            PawnChange change_window = new PawnChange(e.Color == FigureColor.White);
            Point relative_location = _current_clicked_button.TranslatePoint(new Point(0, 0), App.Current.MainWindow);
            change_window.Left = relative_location.X + App.Current.MainWindow.Left + 6;
            change_window.Top = e.Color == FigureColor.White ?
                relative_location.Y + App.Current.MainWindow.Top + 30 :
                relative_location.Y + App.Current.MainWindow.Top + 30 - _current_clicked_button.ActualHeight * 3;
            change_window.Width = _current_clicked_button.ActualWidth + 3;
            change_window.Height = _current_clicked_button.ActualHeight * 4;
            change_window.ShowDialog();
            ChangeResult result = change_window.ChangeResult;
            ResultOfPawnChangeObtained?.Invoke(this, new ResultOfPawnChangeObtainedEventArgs(e.Position, e.Color, result));
        }
    }
}
