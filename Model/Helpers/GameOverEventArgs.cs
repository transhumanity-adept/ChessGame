namespace ChessGame.Model.Helpers
{
    public class GameOverEventArgs
    {
        public GameResult GameResult { get; set; }
        public GameOverEventArgs(GameResult game_result) => GameResult = game_result;
    }
}
