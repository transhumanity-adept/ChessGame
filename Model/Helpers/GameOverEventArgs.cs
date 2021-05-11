using ChessGame.Model.Figures.Helpers;

namespace ChessGame.Model.Helpers
{
    public class GameOverEventArgs
    {
        public FigureColor WinColor { get; set; }
        public GameOverEventArgs(FigureColor win_color) => WinColor = win_color;
    }
}
