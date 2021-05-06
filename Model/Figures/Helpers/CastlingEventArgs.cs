namespace ChessGame.Model.Figures.Helpers
{
    public class CastlingEventArgs
    {
        public Position KingPosition { get; private set; }
        public Position KingMovablePosition { get; private set; }
        public Position RookPosition { get; private set; }
        public Position RookMovablePosition { get; private set; }

        public CastlingEventArgs(Position king_pos, Position king_move_pos, Position rook_pos, Position rook_move_pos)
            => (KingPosition, KingMovablePosition, RookPosition, RookMovablePosition) = (king_pos, king_move_pos, rook_pos, rook_move_pos);
    }
}
