using System;

namespace ChessGame.Model
{
    /// <summary>
    /// Позиция на доске
    /// </summary>
    public struct Position
    {
        #region Поля
        private int _x;
        private int _y;
        #endregion

        #region Конструкторы
        public Position(int x, int y) : this()
        {
            X = x;
            Y = y;
        }
        #endregion

        #region Свойства
        public static int MaxPositionX { get; } = 8;
        public static int MaxPositionY { get; } = 8;
        public int X
        {
            get => _x;
            set
            {
                if (value >= 0 && value < 8) _x = value;
                else throw new Exception("Invalid position x value");
            }
        }
        public int Y
        {
            get => _y;
            set
            {
                if (value >= 0 && value < 8) _y = value;
                else throw new Exception("Invalid position y value");
            }
        }
        #endregion

        #region Операторы
        public static bool operator ==(Position p1, Position p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }
        public static bool operator !=(Position p1, Position p2)
        {
            return p1.X != p2.X || p1.Y != p2.Y;
        }
        #endregion

        #region Методы
        /// <summary>
        /// Информация о позиции в виде строки
        /// </summary>
        /// <returns>Строковое представление позиции</returns>
        public override string ToString()
        {
            return $"{_x}|{_y}";
        }
        #endregion
    }
}
