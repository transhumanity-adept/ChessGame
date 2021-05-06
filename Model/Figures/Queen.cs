using System.Collections.Generic;
using ChessGame.Model.Figures.Helpers;

namespace ChessGame.Model.Figures
{
    class Queen : Figure
    {
        public Queen(Position position, FigureColor color)
            : base(position, color == FigureColor.White ? @"C:\Users\79107\Desktop\ChessGame\Images\wqueen.png" : @"C:\Users\79107\Desktop\ChessGame\Images\bqueen.png", color) { }
        public override List<Position> GetPossibleMoves()
        {
            List<Position> result = new List<Position>();
            for (int i = 0; i < Position.MaxPositionX; i++)
            {
                if (i != _position.X) { result.Add(new Position(i, _position.Y)); }
            }
            for (int i = 0; i < Position.MaxPositionY; i++)
            {
                if(i != _position.Y) { result.Add(new Position(_position.X, i)); }
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
    }
}
