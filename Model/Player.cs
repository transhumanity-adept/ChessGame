using ChessGame.Helpers;

namespace ChessGame.Model
{
    public class Player : NotifyPropertyChanged
    {
        private int _rating;
        public int Rating 
        {
            get => _rating;
            set
            {
                _rating = value;
                OnPropertyChanged();
            }
        }
        public string Login { get; }
        public Player(int rating, string login) => (Rating, Login) = (rating, login);
    }
}
