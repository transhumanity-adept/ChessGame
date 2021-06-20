using System.Collections.Generic;
using ChessGame.Model.Figures.Helpers;
using ChessGame.Helpers;

namespace ChessGame.Model.Figures
{
    /// <summary>
    /// Шахматная фигура "Конь"
    /// </summary>
    class Knight : Figure
    {
        #region Конструкторы
        public Knight(Position position, FigureColor color)
            : base(position, color == FigureColor.White ? RelativePaths.WhiteKnight : RelativePaths.BlackKnight, color) { }
        #endregion

        #region Методы
        /// <summary>
        /// Вычисление возможных ходов фигуры на доске
        /// </summary>
        /// <returns>Коллекция возможных ходов</returns>
        public override List<Position> GetPossibleMoves()
        {
            List<Position> result = new List<Position>();
            try { result.Add(new Position(_position.X - 1, _position.Y - 2)); } catch { }
            try { result.Add(new Position(_position.X - 2, _position.Y - 1)); } catch { }
            try { result.Add(new Position(_position.X - 2, _position.Y + 1)); } catch { }
            try { result.Add(new Position(_position.X - 1, _position.Y + 2)); } catch { }
            try { result.Add(new Position(_position.X + 1, _position.Y - 2)); } catch { }
            try { result.Add(new Position(_position.X + 2, _position.Y - 1)); } catch { }
            try { result.Add(new Position(_position.X + 2, _position.Y + 1)); } catch { }
            try { result.Add(new Position(_position.X + 1, _position.Y + 2)); } catch { }
            return result;
        }
        #endregion
    }
}
