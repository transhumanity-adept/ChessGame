using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ChessGame.Model;
using ChessGame.Model.Figures;
using ChessGame.Helpers;
using ChessGame.Model.Figures.Helpers;
using ChessGame.View;
using ChessGame.Model.Helpers;
using System;
using System.Windows;

namespace ChessGame.ViewModel
{
    public class CellClickedEventArgs
    {
        public Cell ClickedCell { get; private set; }
        public CellClickedEventArgs(Cell clicked_cell) => ClickedCell = clicked_cell;
    }

    public class ResultOfPawnChangeObtainedEventArgs
    {
        public Position FigurePosition { get; private set; }
        public FigureColor FigureColor { get; private set; }
        public ChangeResult ChangeResult { get; private set; }
        public ResultOfPawnChangeObtainedEventArgs(Position position, FigureColor color, ChangeResult result)
            => (FigurePosition, FigureColor, ChangeResult) = (position, color, result);
    }
    class ChessViewModel : NotifyPropertyChanged
    {
        private Board _board;
        private ICommand _cell_click_command;
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
            Cell clicked_cell = (Cell)obj;
            ClickedOnCell?.Invoke(this, new CellClickedEventArgs(clicked_cell));
        }, obj => obj is Cell));

        public ChessViewModel()
        {
            _board = new Board(this);
            _board.PawnChanged += Board_PawnChanged;
        }

        private void Board_PawnChanged(object sender, PawnChangedEventArgs e)
        {
            PawnChange change_window = new PawnChange(e.Color == FigureColor.White ? true: false);
            change_window.Left += e.Position.Y * 76;
            change_window.ShowDialog();
            ChangeResult result = change_window.ChangeResult;
            ResultOfPawnChangeObtained?.Invoke(this, new ResultOfPawnChangeObtainedEventArgs(e.Position, e.Color, result));
        }
    }
}
