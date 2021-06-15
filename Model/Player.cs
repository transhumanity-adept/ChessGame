namespace ChessGame.Model
{
    public class Player
    {
        public int Rating { get; set; }
        public string Login { get; set; }
        public Player(int rating, string login) => (Rating, Login) = (rating, login);
    }
}
