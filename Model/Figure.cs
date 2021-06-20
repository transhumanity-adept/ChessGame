using System;
using System.Collections.Generic;
using ChessGame.Model.Figures.Helpers;

namespace ChessGame.Model
{
    /// <summary>
    /// Фигура доски
    /// </summary>
    public abstract class Figure
    {
        #region Поля
        protected Position _position;
        protected string _image_path;
        protected FigureColor _color;
        #endregion

        #region Конструкторы
        public Figure(Position position, string image, FigureColor color)
            => (Position, ImagePath, Color) = (position, image, color);
        #endregion

        #region Свойства
        public Position Position { get => _position; set => _position = value; }
        public string ImagePath
        {
            get => _image_path;
            private set => _image_path = value;
        }
        public FigureColor Color
        {
            get => _color;
            private set => _color = value;
        }
        #endregion

        #region События
        public event Action<object, Position, Position> Moved;
        public event Action<object, Position, Position> Attacked;
        #endregion

        #region Методы
        /// <summary>
        /// Ход фигуры
        /// </summary>
        /// <param name="new_position">Новая позиция</param>
        /// <param name="count_moves">Количество ходов на доске</param>
        public virtual void MoveTo(Position new_position, int count_moves)
        {
            Position last_pos = Position;
            Position = new_position;
            Moved?.Invoke(this, last_pos, new_position);
        }
        /// <summary>
        /// Атака на клетку доски
        /// </summary>
        /// <param name="attack_position">Атакуемая позиция на доске</param>
        public virtual void AttackTo(Position attack_position)
        {
            Position last_pos = Position;
            Position = attack_position;
            Attacked?.Invoke(this, last_pos, attack_position);
        }
        /// <summary>
        /// Вычисление возможных ходов фигуры на доске
        /// </summary>
        /// <returns>Коллекция возможных ходов</returns>
        public abstract List<Position> GetPossibleMoves();
        /// <summary>
        /// Информация о фигуре в виде строки
        /// </summary>
        /// <returns>Строковое представление фигуры</returns>
        public override string ToString()
        {
            return $"{GetType().Name} {Position} {Color}";
        }
        #endregion
    }
}
