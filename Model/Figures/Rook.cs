using System.Collections.Generic;
using ChessGame.Model.Figures.Helpers;
using ChessGame.Helpers;

namespace ChessGame.Model.Figures
{
    /// <summary>
    /// Шахматная фигура "Ладья"
    /// </summary>
    class Rook : Figure
    {
        #region Конструкторы
        public Rook(Position position, FigureColor color)
            : base(position, color == FigureColor.White ? RelativePaths.WhiteRook : RelativePaths.BlackRook, color)
        {
            MovementsState = MovementsState.Zero;
        }
        #endregion

        #region Свойства
        public MovementsState MovementsState { get; private set; }
        #endregion

        #region Методы
        /// <summary>
        /// Ход фигуры
        /// </summary>
        /// <param name="new_position">Новая позиция</param>
        /// <param name="count_moves">Количество ходов на доске</param>
        public override void MoveTo(Position new_position, int count_moves)
        {
            switch (MovementsState)
            {
                case MovementsState.Zero: { MovementsState = MovementsState.One; break; }
                case MovementsState.One: { MovementsState = MovementsState.More; break; }
            }
            base.MoveTo(new_position, count_moves);
        }
        /// <summary>
        /// Вычисление возможных ходов фигуры на доске
        /// </summary>
        /// <returns>Коллекция возможных ходов</returns>
        public override List<Position> GetPossibleMoves()
        {
            List<Position> result = new List<Position>();
            for (int i = 0; i < Position.MaxPositionX; i++)
            {
                if (i != _position.X) { result.Add(new Position(i, _position.Y)); }
            }
            for (int i = 0; i < Position.MaxPositionY; i++)
            {
                if (i != _position.Y) { result.Add(new Position(_position.X, i)); }
            }
            return result;
        }
        #endregion
    }
}
