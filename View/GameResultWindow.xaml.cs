using ChessGame.Model.Helpers;
using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace ChessGame.View
{
    /// <summary>
    /// Логика взаимодействия для GameResultWindow.xaml
    /// </summary>
    public partial class GameResultWindow : Window
    {
        #region Поля
        private readonly Duration _duration_for_font_size = new Duration(TimeSpan.FromSeconds(0.5));
        private readonly Duration _duration_for_open_opacity = new Duration(TimeSpan.FromSeconds(0.5));
        private readonly Duration _duration_for_close_opacity = new Duration(TimeSpan.FromSeconds(0.5));
        #endregion

        #region Конструкторы
        public GameResultWindow()
        {
            InitializeComponent();
        }
        public GameResultWindow(GameResult result) : this()
        {
            Owner = App.Current.MainWindow;
            (Width, Height, Left, Top) = (Owner.ActualWidth, Owner.ActualHeight, Owner.Left, Owner.Top);
            switch (result)
            {
                case GameResult.BlackWin: { _text_block.Text = "ПОБЕДИЛА\nКОМАНДА\nЧЕРНЫХ"; break; }
                case GameResult.WhiteWin: { _text_block.Text = "ПОБЕДИЛА\nКОМАНДА\nБЕЛЫХ"; break; }
                default: { _text_block.Text = "Ничья"; break; }
            }
        }

        #endregion

        #region Методы
        /// <summary>
        /// Анимация основной текстовой информации окна
        /// </summary>
        private void OpenAnimation()
        {
            DoubleAnimation font_size_animation = new DoubleAnimation(35,60,_duration_for_font_size);
            DoubleAnimation opacity_animation = new DoubleAnimation(0, 1, _duration_for_open_opacity);
            opacity_animation.Completed += TextBlockShowAnimationCompleted;
            _text_block.BeginAnimation(FontSizeProperty, font_size_animation);
            _text_block.BeginAnimation(OpacityProperty, opacity_animation);
        }
        /// <summary>
        /// Обработчик события "Анимация прозрачности завершена"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void TextBlockShowAnimationCompleted(object sender, EventArgs e)
        {
            _button_close.IsEnabled = true;
        }
        /// <summary>
        /// Анимация закрытия окна
        /// </summary>
        private void CloseAnimation()
        {
            DoubleAnimation opacity_animation = new DoubleAnimation(0, _duration_for_close_opacity);
            opacity_animation.Completed += TextBlockHideAnimationCompleted;
            _text_block.BeginAnimation(OpacityProperty, opacity_animation);
        }
        /// <summary>
        /// Обработчик события "Анимация прозрачности завершена"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void TextBlockHideAnimationCompleted(object sender, EventArgs e)
        {
            Close();
        }
        /// <summary>
        /// Обработчик события "Окно загружено"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            OpenAnimation();
        }
        /// <summary>
        /// Обработчик события "Клин по кнопке "ОК""
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            _button_close.IsEnabled = false;
            CloseAnimation();
        }
        #endregion
    }
}
