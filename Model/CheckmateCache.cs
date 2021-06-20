using System.Collections.Generic;

namespace ChessGame.Model
{
    /// <summary>
    /// Кэш проверки на мат
    /// </summary>
    static class CheckmateCache
    {
        #region Свойства
        /// <summary>
        /// Кэшированные возможные ходы всех фигур на доске
        /// </summary>
        public static List<(Figure figure, List<Position> possible_moves)> CachedMoves { get; set; } = new List<(Figure figure, List<Position> possible_moves)>();

        #endregion

        #region Методы
        /// <summary>
        /// Очистить кэш
        /// </summary>
        public static void Clear()
        {
            CachedMoves.Clear();
        }

        #endregion
    }
}
