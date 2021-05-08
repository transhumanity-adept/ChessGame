using ChessGame.Helpers;
using ChessGame.Model;
using ChessGame.Model.Figures.Helpers;

namespace ChessGame.ViewModel.Helpers
{
    public class ResultOfPawnChangeObtainedEventArgs
    {
        public Position FigurePosition { get; private set; }
        public FigureColor FigureColor { get; private set; }
        public ChangeResult ChangeResult { get; private set; }
        public ResultOfPawnChangeObtainedEventArgs(Position position, FigureColor color, ChangeResult result)
            => (FigurePosition, FigureColor, ChangeResult) = (position, color, result);
    }
}
