using System.Windows;
using System.Windows.Controls;

namespace ChessGame.View
{
    /// <summary>
    /// Логика взаимодействия для PawnChange.xaml
    /// </summary>
    public partial class PawnChange : Window
    {
        public ChangeResult ChangeResult { get; private set; }
        public PawnChange()
        {
            InitializeComponent();
        }

        public PawnChange(bool is_white) : this()
        {
            Image queen_image = button_queen.Content as Image;
            if (is_white)
            {

            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public enum ChangeResult
    {
        Queen,
        Bishop,
        Knight,
        Rook
    }
}
