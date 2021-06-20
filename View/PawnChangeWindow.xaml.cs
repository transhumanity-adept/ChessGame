using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ChessGame.Helpers;

namespace ChessGame.View
{
    /// <summary>
    /// Логика взаимодействия для PawnChangeWindow.xaml
    /// </summary>
    public partial class PawnChangeWindow : Window
    {
        #region Поля
        private readonly Duration _duration = new Duration(TimeSpan.FromSeconds(1));
        private double _default_height;
        #endregion

        #region Конструкторы
        public PawnChangeWindow()
        {
            InitializeComponent();
        }
        public PawnChangeWindow(bool is_white) : this()
        {
            Image queen_image = _button_queen.Content as Image;
            Image bishop_image = _button_bishop.Content as Image;
            Image knight_image = _button_knight.Content as Image;
            Image rook_image = _button_rook.Content as Image;

            if (queen_image == null || bishop_image == null || knight_image == null || rook_image == null) return;
            ImageSourceConverter image_source_converter = new ImageSourceConverter();
            if (is_white)
            {
                queen_image.Source = (ImageSource)image_source_converter.ConvertFromString(RelativePaths.WhiteQueen);
                bishop_image.Source = (ImageSource)image_source_converter.ConvertFromString(RelativePaths.WhiteBishop);
                knight_image.Source = (ImageSource)image_source_converter.ConvertFromString(RelativePaths.WhiteKnight);
                rook_image.Source = (ImageSource)image_source_converter.ConvertFromString(RelativePaths.WhiteRook);
                _border.CornerRadius = new CornerRadius(0, 0, 25, 25);
            }
            else
            {
                queen_image.Source = (ImageSource)image_source_converter.ConvertFromString(RelativePaths.BlackQueen);
                bishop_image.Source = (ImageSource)image_source_converter.ConvertFromString(RelativePaths.BlackBishop);
                knight_image.Source = (ImageSource)image_source_converter.ConvertFromString(RelativePaths.BlackKnight);
                rook_image.Source = (ImageSource)image_source_converter.ConvertFromString(RelativePaths.BlackRook);
                _border.CornerRadius = new CornerRadius(25, 25, 0, 0);
            }
        }
        #endregion

        #region Свойства
        public ChangeResult ChangeResult { get; private set; }
        #endregion

        #region Методы
        /// <summary>
        /// Обработчик события "Окно загружено"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            _default_height = ActualHeight;
            OpenAnimation();
        }
        /// <summary>
        /// Анимация показа окна
        /// </summary>
        private void OpenAnimation()
        {
            DoubleAnimation height_animation = new DoubleAnimation(1, _default_height, _duration);
            BeginAnimation(HeightProperty, height_animation);
        }
        /// <summary>
        /// Анимация закрытия окна
        /// </summary>
        private void CloseAnimation()
        {
            DoubleAnimation height_animation = new DoubleAnimation(1, _duration);
            height_animation.Completed += CloseAnimationCompleted;
            BeginAnimation(HeightProperty, height_animation);
        }
        /// <summary>
        /// Обработчик события "Окно загружено"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void CloseAnimationCompleted(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                DialogResult = false;
                Close();
            });
        }
        /// <summary>
        /// Обработчик события "Клик по кнопке окна"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button)) return;
            switch((sender as Button).Name)
            {
                case "_button_queen": { ChangeResult = ChangeResult.Queen; break; }
                case "_button_bishop": { ChangeResult = ChangeResult.Bishop; break; }
                case "_button_knight": { ChangeResult = ChangeResult.Knight; break; }
                case "_button_rook": { ChangeResult = ChangeResult.Rook; break; }
            }
            CloseAnimation();
        }
        #endregion
    }
}
