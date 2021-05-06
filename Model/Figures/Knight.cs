﻿using System.Collections.Generic;
using ChessGame.Model.Figures.Helpers;

namespace ChessGame.Model.Figures
{
    class Knight : Figure
    {
        public Knight(Position position, FigureColor color)
            : base(position, color == FigureColor.White ? @"..\..\Images\wknight.png" : @"..\..\Images\bknight.png", color) { }
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
    }
}
