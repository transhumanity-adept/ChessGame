using System.Collections.Generic;
using ChessGame.Model.Figures.Helpers;
using ChessGame.Helpers;

namespace ChessGame.Model.Figures
{
    /// <summary>
    /// Шахматная фигура "Ферзь"
    /// </summary>
    class Queen : Figure
    {
        #region Конструкторы
        public Queen(Position position, FigureColor color)
            : base(position, color == FigureColor.White ? RelativePaths.WhiteQueen : RelativePaths.BlackQueen, color) { }
        #endregion

        #region Методы
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
            for (int i = 0; i < Position.MaxPositionX; i++)
            {
                for (int j = 0; j < Position.MaxPositionY; j++)
                {
                    if ((i + j == Position.X + Position.Y || i - j == Position.X - Position.Y) && (i != Position.X && j != Position.Y))
                        result.Add(new Position(i, j));
                }
            }
            return result;
        }

        #endregion
    }
}
