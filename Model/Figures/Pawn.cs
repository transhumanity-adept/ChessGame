using System;
using System.Collections.Generic;
using ChessGame.Model.Figures.Helpers;
using ChessGame.Helpers;

namespace ChessGame.Model.Figures
{
    /// <summary>
    /// Шахматная фигура "Пешка"
    /// </summary>
    public class Pawn : Figure
    {
        #region Конструкторы
        public Pawn(Board board, Position position, FigureColor color)
            : base(position, color == FigureColor.White ? RelativePaths.WhitePawn : RelativePaths.BlackPawn, color)
        {
            MovementsState = MovementsState.Zero;
            board.EnPassantChanged += BoardEnPassantChanged;
        }

        private void BoardEnPassantChanged(object sender, Pawn pawn, bool en_passant)
        {
            if (pawn == this) HasEnPassant = en_passant;
        }

        public Pawn(Board board, Position position, FigureColor color, MovementsState movement_state, bool has_en_passant, int en_passant_number_move)
            : base(position, color == FigureColor.White ? RelativePaths.WhitePawn : RelativePaths.BlackPawn, color)
        {
            MovementsState = movement_state;
            HasEnPassant = has_en_passant;
            EnPassantNumberMove = en_passant_number_move;
        }
        #endregion

        #region Свойства
        public MovementsState MovementsState { get; private set; }
        public bool HasEnPassant { get; private set; }
        public int EnPassantNumberMove { get; private set; }
        #endregion

        #region События
        public event Action<object, Position, Position, Position> EnPassanted;
        public event Action<object> Changed;
        #endregion

        #region Методы
        /// <summary>
        /// Атака на клетку доски
        /// </summary>
        /// <param name="attack_position">Атакуемая позиция на доске</param>
        public override void AttackTo(Position attack_position)
        {
            base.AttackTo(attack_position);
            if (Position.X == (_color == FigureColor.White ? 7 : 0))
            {
                Changed?.Invoke(this);
            }
        }
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
            if (Math.Abs(new_position.X - _position.X) == 2) EnPassantNumberMove = count_moves;
            base.MoveTo(new_position, count_moves);
            if (Position.X == (_color == FigureColor.White ? 7 : 0))
            {
                Changed?.Invoke(this);
            }
        }
        /// <summary>
        /// Атака с взятием на проходе
        /// </summary>
        /// <param name="en_passanted_figure_position">Позиция атакуемой фигуры</param>
        public void EnPassantAttack(Position en_passanted_figure_position)
        {
            Position last_pos = Position;
            Position = Color == FigureColor.White ?
                new Position(en_passanted_figure_position.X + 1, en_passanted_figure_position.Y) :
                new Position(en_passanted_figure_position.X - 1, en_passanted_figure_position.Y);
            EnPassanted?.Invoke(this, last_pos, en_passanted_figure_position, Position);
        }
        /// <summary>
        /// Вычисление возможных ходов фигуры на доске
        /// </summary>
        /// <returns>Коллекция возможных ходов</returns>
        public override List<Position> GetPossibleMoves()
        {
            List<Position> result = new List<Position>();
            if (_color == FigureColor.White)
            {
                if (Position.X != 7)
                {
                    result.Add(new Position(Position.X + 1, Position.Y));
                }
            }
            else
            {
                if (Position.X != 0)
                {
                    result.Add(new Position(Position.X - 1, Position.Y));
                }
            }
            return result;
        }
        /// <summary>
        /// Информация о фигуре в виде строки
        /// </summary>
        /// <returns>Строковое представление фигуры</returns>
        public override string ToString()
        {
            return $"{base.ToString()} {MovementsState} {HasEnPassant} {EnPassantNumberMove}";
        }
        #endregion
    }
}
