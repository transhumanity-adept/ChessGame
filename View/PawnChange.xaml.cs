using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ChessGame.Helpers;

namespace ChessGame.View
{
    /// <summary>
    /// Логика взаимодействия для PawnChange.xaml
    /// </summary>
    public partial class PawnChange : Window
    {
        public ChangeResult ChangeResult { get; private set; }
        private readonly Duration _duration = new Duration(TimeSpan.FromSeconds(1));
        private double _default_height;
        public PawnChange()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _default_height = ActualHeight;
            OpenAnimation();
        }

        private void OpenAnimation()
        {
            DoubleAnimation height_animation = new DoubleAnimation(1, _default_height, _duration);
            BeginAnimation(HeightProperty, height_animation);
        }

        private void CloseAnimation()
        {
            DoubleAnimation height_animation = new DoubleAnimation(1, _duration);
            height_animation.Completed += Height_animation_Completed;
            BeginAnimation(HeightProperty, height_animation);
        }

        private void Height_animation_Completed(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                DialogResult = false;
                Close();
            });
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
                border.CornerRadius = new CornerRadius(0, 0, 25, 25);
            }
            else
            {
                queen_image.Source = (ImageSource)image_source_converter.ConvertFromString(ImagePaths.BlackQueen);
                bishop_image.Source = (ImageSource)image_source_converter.ConvertFromString(ImagePaths.BlackBishop);
                knight_image.Source = (ImageSource)image_source_converter.ConvertFromString(ImagePaths.BlackKnight);
                rook_image.Source = (ImageSource)image_source_converter.ConvertFromString(ImagePaths.BlackRook);
                border.CornerRadius = new CornerRadius(25, 25, 0, 0);
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
            CloseAnimation();
        }
    }
}
