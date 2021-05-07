using System;
using System.Collections.Generic;
using ChessGame.Model.Figures.Helpers;
using ChessGame.Helpers;

namespace ChessGame.Model.Figures
{
    class Pawn : Figure
    {
        public MovementsState MovementsState { get; private set; }
        public bool HasEnPassant { get; set; }
        public int EnPassantNumberMove { get; set; }
        public Pawn(Position position, FigureColor color) 
            : base(position, color == FigureColor.White ? ImagePaths.WhitePawn : ImagePaths.BlackPawn, color)
        {
            MovementsState = MovementsState.Zero;
        }

        public delegate void EnPassantHandler(object sender, EnPassantedEventArgs e);
        public event EnPassantHandler EnPassanted;
        public event EventHandler Changed;

        public override void AttackTo(Position attack_position)
        {
            base.AttackTo(attack_position);
            if (Position.X == (_color == FigureColor.White ? 7 : 0))
            {
                Changed?.Invoke(this, new EventArgs());
            }
        }
        public override void MoveTo(Position new_position, int count_moves)
        {
            switch (MovementsState)
            {
                case MovementsState.Zero : { MovementsState = MovementsState.One; break; }
                case MovementsState.One: { MovementsState = MovementsState.More; break; }
            }
            if (Math.Abs(new_position.X - _position.X) == 2) EnPassantNumberMove = count_moves;
            base.MoveTo(new_position, count_moves);
            if (Position.X == (_color == FigureColor.White ? 7 : 0))
            {
                Changed?.Invoke(this, new EventArgs());
            }
        }

        public void EnPassantAttack(Position en_passanted_figure_position)
        {
            Position last_pos = Position;
            Position = Color == FigureColor.White ?
                new Position(en_passanted_figure_position.X + 1, en_passanted_figure_position.Y) :
                new Position(en_passanted_figure_position.X - 1, en_passanted_figure_position.Y);
            EnPassanted?.Invoke(this, new EnPassantedEventArgs(last_pos, en_passanted_figure_position, Position));
        }

        public override List<Position> GetPossibleMoves()
        {
            List<Position> result = new List<Position>();
            if(_color == FigureColor.White)
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
    }
}
