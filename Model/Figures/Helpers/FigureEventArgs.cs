namespace ChessGame.Model.Figures.Helpers
{
    public class FigureEventArgs
    {
        private Position _moved_from;
        private Position _moved_to;

        public FigureEventArgs(Position movedFrom, Position movedTo)
            => (MovedFrom, MovedTo) = (movedFrom, movedTo);

        public Position MovedFrom { get => _moved_from; set => _moved_from = value; }
        public Position MovedTo { get => _moved_to; set => _moved_to = value; }
    }
}
