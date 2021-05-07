using ChessGame.Model.Figures.Helpers;

namespace ChessGame.Model.Helpers
{
    class PawnChangedEventArgs
    {
        public Position Position { get; private set; }
        public FigureColor Color { get; private set; }

        public PawnChangedEventArgs(Position position, FigureColor color) => (Position, Color) = (position, color);
    }
}
