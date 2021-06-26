using System.Collections.Generic;
using ChessGame.Model.Figures.Helpers;
using ChessGame.Helpers;
using System;

namespace ChessGame.Model.Figures
{
    /// <summary>
    /// Шахматная фигура "Король"
    /// </summary>
    public class King : Figure
    {
        #region Конструкторы
        public King(Board board, Position position, FigureColor color)
            : base(position, color == FigureColor.White ? RelativePaths.WhiteKing : RelativePaths.BlackKing, color) 
        {
            MovementsState = MovementsState.Zero;
            board.CastleChanged += BoardCastleChanged;
            board.Castled += BoardCastled;
        }
        public King(Board board, Position position, FigureColor color, MovementsState movement_state, bool has_castle)
            : base(position, color == FigureColor.White ? RelativePaths.WhiteKing : RelativePaths.BlackKing, color)
        {
            MovementsState = movement_state;
            HasCastle = has_castle;
        }
        #endregion

        #region Свойства
        public MovementsState MovementsState { get; private set; }
        public bool HasCastle { get; private set; }
        #endregion

        #region События
        /// <summary>
        /// Событие "Рокировка"
        /// </summary>
        public event Action<object, Position, Position, Position, Position> ToCastled;
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
            HasCastle = false;
            base.MoveTo(new_position, count_moves);
        }
        /// <summary>
        /// Рокировка
        /// </summary>
        /// <param name="king_move_pos">Допустимые ходы короля</param>
        /// <param name="rook_pos">Позиция ладьи</param>
        /// <param name="rook_move_pos">Допустимые ходы ладьи</param>
        public void ToCastle(Position king_move_pos, Position rook_pos, Position rook_move_pos)
        {
            Position last_pos = Position;
            Position = king_move_pos;
            ToCastled?.Invoke(this, last_pos, king_move_pos, rook_pos, rook_move_pos);
        }
        /// <summary>
        /// Вычисление возможных ходов фигуры на доске
        /// </summary>
        /// <returns>Коллекция возможных ходов</returns>
        public override List<Position> GetPossibleMoves()
        {
            List<Position> result = new List<Position>();
            try { result.Add(new Position(_position.X + 0, _position.Y + 1)); } catch { }
            try { result.Add(new Position(_position.X + 0, _position.Y - 1)); } catch { }
            try { result.Add(new Position(_position.X + 1, _position.Y + 0)); } catch { }
            try { result.Add(new Position(_position.X - 1, _position.Y + 0)); } catch { }
            try { result.Add(new Position(_position.X - 1, _position.Y - 1)); } catch { }
            try { result.Add(new Position(_position.X - 1, _position.Y + 1)); } catch { }
            try { result.Add(new Position(_position.X + 1, _position.Y - 1)); } catch { }
            try { result.Add(new Position(_position.X + 1, _position.Y + 1)); } catch { }
            return result;
        }
        private void BoardCastled(object sender, King king, Position king_to_pos, Rook rook, Position rook_to_pos)
        {
            if (king == this) HasCastle = false;
        }
        private void BoardCastleChanged(object sender, King king, bool castle)
        {
            if (king == this) HasCastle = castle;
        }
        /// <summary>
        /// Информация о фигуре в виде строки
        /// </summary>
        /// <returns>Строковое представление фигуры</returns>
        public override string ToString()
        {
            return $"{base.ToString()} {MovementsState} {HasCastle}";
        }
        #endregion
    }
}
