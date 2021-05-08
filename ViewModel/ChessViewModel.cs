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
using System.Windows.Media;

namespace ChessGame.ViewModel
{
    class ChessViewModel : NotifyPropertyChanged
    {
        private Board _board;
        private ICommand _cell_click_command;
        private Button _current_clicked_button;
        public delegate void ClickedCellHandled(object sender, CellClickedEventArgs e);
        public event ClickedCellHandled ClickedOnCell;
        public delegate void ResultOfPawnChangeObtainedHandler(object sender, ResultOfPawnChangeObtainedEventArgs e);
        public event ResultOfPawnChangeObtainedHandler ResultOfPawnChangeObtained;

        public Board Board
        {
            get => _board;
            set
            {
                _board = value;
                OnPropertyChanged();
            }
        }

        public ICommand CellClickCommand => _cell_click_command ?? (_cell_click_command = new RelayCommand(obj =>
        {
            var (button, cell) = obj as Tuple<Button, Cell>;
            _current_clicked_button = button;
            ClickedOnCell?.Invoke(this, new CellClickedEventArgs(cell));
        }));

        public ChessViewModel()
        {
            _board = new Board(this);
            _board.PawnChanged += Board_PawnChanged;
        }

        private void Board_PawnChanged(object sender, PawnChangedEventArgs e)
        {
            PawnChange change_window = new PawnChange(e.Color == FigureColor.White);
            Point y = _current_clicked_button.TranslatePoint(new Point(0, 0), App.Current.MainWindow);
            change_window.Left = y.X + App.Current.MainWindow.Left + 7.4;
            change_window.Top = e.Color == FigureColor.White ?
                y.Y + App.Current.MainWindow.Top + 30 : 
                y.Y + App.Current.MainWindow.Top + 30 - _current_clicked_button.ActualHeight * 3;
            change_window.Width = _current_clicked_button.ActualWidth;
            change_window.Height = _current_clicked_button.ActualHeight * 4;
            change_window.ShowDialog();
            ChangeResult result = change_window.ChangeResult;
            ResultOfPawnChangeObtained?.Invoke(this, new ResultOfPawnChangeObtainedEventArgs(e.Position, e.Color, result));
        }
    }
}
