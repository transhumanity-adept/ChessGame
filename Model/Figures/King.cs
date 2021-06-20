using System.Collections.Generic;
using ChessGame.Model.Figures.Helpers;
using ChessGame.Helpers;

namespace ChessGame.Model.Figures
{
    class King : Figure
    {
        public King(Position position, FigureColor color)
            : base(position, color == FigureColor.White ? RelativePaths.WhiteKing : RelativePaths.BlackKing, color) 
        {
            MovementsState = MovementsState.Zero;
        }

        public King(Position position, FigureColor color, MovementsState movement_state, bool has_castle)
            : base(position, color == FigureColor.White ? RelativePaths.WhiteKing : RelativePaths.BlackKing, color)
        {
            MovementsState = movement_state;
            HasCastle = has_castle;
        }
        public delegate void CastlingHandler(object sender, CastlingEventArgs e);
        public event CastlingHandler ToCastled;
        public MovementsState MovementsState { get; private set; }
        public bool HasCastle { get; set; }
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
        public void ToCastle(Position king_move_pos, Position rook_pos, Position rook_move_pos)
        {
            Position last_pos = Position;
            Position = king_move_pos;
            ToCastled?.Invoke(this, new CastlingEventArgs(last_pos, king_move_pos, rook_pos, rook_move_pos));
        }
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
        public override string ToString()
        {
            return $"{base.ToString()} {MovementsState} {HasCastle}";
        }
    }
}
