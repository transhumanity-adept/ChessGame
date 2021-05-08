using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ChessGame.Helpers;

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
            Image bishop_image = button_bishop.Content as Image;
            Image knight_image = button_knight.Content as Image;
            Image rook_image = button_rook.Content as Image;

            if (queen_image == null || bishop_image == null || knight_image == null || rook_image == null) return;
            ImageSourceConverter image_source_converter = new ImageSourceConverter();
            if (is_white)
            {
                queen_image.Source = (ImageSource)image_source_converter.ConvertFromString(ImagePaths.WhiteQueen);
                bishop_image.Source = (ImageSource)image_source_converter.ConvertFromString(ImagePaths.WhiteBishop);
                knight_image.Source = (ImageSource)image_source_converter.ConvertFromString(ImagePaths.WhiteKnight);
                rook_image.Source = (ImageSource)image_source_converter.ConvertFromString(ImagePaths.WhiteRook);
            }
            else
            {
                queen_image.Source = (ImageSource)image_source_converter.ConvertFromString(ImagePaths.BlackQueen);
                bishop_image.Source = (ImageSource)image_source_converter.ConvertFromString(ImagePaths.BlackBishop);
                knight_image.Source = (ImageSource)image_source_converter.ConvertFromString(ImagePaths.BlackKnight);
                rook_image.Source = (ImageSource)image_source_converter.ConvertFromString(ImagePaths.BlackRook);
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button)) return;
            switch((sender as Button).Name)
            {
                case "button_queen": { ChangeResult = ChangeResult.Queen; break; }
                case "button_bishop": { ChangeResult = ChangeResult.Bishop; break; }
                case "button_knight": { ChangeResult = ChangeResult.Knight; break; }
                case "button_rook": { ChangeResult = ChangeResult.Rook; break; }
            }
            Close();
        }
    }
}
