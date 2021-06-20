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
        public Cell(CellColors color, bool active, Position position)
            => (Color, Active, Position) = (color, active, position);
        #endregion

        #region Свойства
        public bool Active
        {
            get => _active;
            set
            {
                _active = value;
                OnPropertyChanged();
            }
        }
        public CellColors Color
        {
            get => _color;
            set
            {
                _color = value;
                OnPropertyChanged();
            }
        }
        public bool IsPossible
        {
            get => _is_possible;
            set
            {
                _is_possible = value;
                OnPropertyChanged();
            }
        }
        public Figure Figure
        {
            get => _figure;
            set
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
