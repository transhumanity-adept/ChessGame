using System.Collections.Generic;
using ChessGame.Model.Figures.Helpers;
using ChessGame.Helpers;

namespace ChessGame.Model.Figures
{
    class Bishop : Figure
    {
        public Bishop(Position position, FigureColor color)
            : base(position, color == FigureColor.White ? ImagePaths.WhiteBishop : ImagePaths.BlackBishop, color) { }

        public override List<Position> GetPossibleMoves()
        {
            List<Position> result = new List<Position>();
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
    }
}
