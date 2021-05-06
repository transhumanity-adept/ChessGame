using System.Collections.Generic;
using ChessGame.Model.Figures.Helpers;

namespace ChessGame.Model.Figures
{
    class Rook : Figure
    {
        public Rook(Position position, FigureColor color)
            : base(position, color == FigureColor.White ? @"C:\Users\79107\Desktop\ChessGame\Images\wrook.png" : @"C:\Users\79107\Desktop\ChessGame\Images\brook.png", color) 
        {
            MovementsState = MovementsState.Zero;
        }
        public MovementsState MovementsState { get; private set; }

        public override void MoveTo(Position new_position, int count_moves)
        {
            switch (MovementsState)
            {
                case MovementsState.Zero: { MovementsState = MovementsState.One; break; }
                case MovementsState.One: { MovementsState = MovementsState.More; break; }
            }
            base.MoveTo(new_position, count_moves);
        }
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
    }
}
