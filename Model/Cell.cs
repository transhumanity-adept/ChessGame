using ChessGame.Helpers;
using ChessGame.Model.Helpers;

namespace ChessGame.Model
{
    /// <summary>
    /// Клетка доски
    /// </summary>
    public class Cell : NotifyPropertyChanged
    {
        #region Поля
        private bool _active;
        private CellColors _color;
        private Figure _figure;
        private Position _position;
        private bool _is_possible;
        #endregion

        #region Конструкторы
        public Cell(Board board, CellColors color, bool active, Position position)
        {
            (Color, Active, Position) = (color, active, position);
            board.CellActivationChanged += BoardCellActivationChanged;
            board.CellBecamePossible += BoardCellBecamePossible;
            board.CellsPossbleReset += BoardCellsPossbleReset;
            board.FigureOnCellChanged += BoardFigureOnCellChanged;
        }

        private void BoardFigureOnCellChanged(object sender, Cell cell, Figure figure)
        {
            if (cell == this) Figure = figure;
        }

        private void BoardCellsPossbleReset(object sender)
        {
            IsPossible = false;
        }

        private void BoardCellBecamePossible(object sender, Cell cell)
        {
            if (cell == this) IsPossible = true;
        }

        private void BoardCellActivationChanged(object sender, Cell cell, bool active)
        {
            if (cell == this) Active = active;
        }
        #endregion

        #region Свойства
        public bool Active
        {
            get => _active;
            private set
            {
                _active = value;
                OnPropertyChanged();
            }
        }
        public CellColors Color
        {
            get => _color;
            private set
            {
                _color = value;
                OnPropertyChanged();
            }
        }
        public bool IsPossible
        {
            get => _is_possible;
            private set
            {
                _is_possible = value;
                OnPropertyChanged();
            }
        }
        public Figure Figure
        {
            get => _figure;
            private set
            {
                _figure = value;
                OnPropertyChanged();
            }
        }
        public Position Position
        {
            get => _position;
            private set => _position = value;
        }
        #endregion
    }
}
