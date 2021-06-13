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
        private readonly Duration _duration_for_font_size = new Duration(TimeSpan.FromSeconds(0.5));
        private readonly Duration _duration_for_open_opacity = new Duration(TimeSpan.FromSeconds(0.5));
        private readonly Duration _duration_for_close_opacity = new Duration(TimeSpan.FromSeconds(0.5));
        public GameResultWindow()
        {
            InitializeComponent();
        }

        public GameResultWindow(GameResult result, Window owner) : this()
        {
            Owner = owner;
            (Width, Height, Left, Top) = (owner.ActualWidth, owner.ActualHeight, owner.Left, owner.Top);
            switch (result)
            {
                case GameResult.BlackWin: { text_block.Text = "Победила\nкоманда\nчерных"; break; }
                case GameResult.WhiteWin: { text_block.Text = "Победила\nкоманда\nбелых"; break; }
                default: { text_block.Text = "Ничья"; break; }
            }
        }

        private void OpenAnimation()
        {
            DoubleAnimation font_size_animation = new DoubleAnimation(35,75,_duration_for_font_size);
            DoubleAnimation opacity_animation = new DoubleAnimation(0, 1, _duration_for_open_opacity);
            opacity_animation.Completed += Opacity_animation_Completed1;
            text_block.BeginAnimation(FontSizeProperty, font_size_animation);
            text_block.BeginAnimation(OpacityProperty, opacity_animation);
        }

        private void Opacity_animation_Completed1(object sender, EventArgs e)
        {
            button_close.IsEnabled = true;
        }

        private void CloseAnimation()
        {
            DoubleAnimation opacity_animation = new DoubleAnimation(0, _duration_for_close_opacity);
            opacity_animation.Completed += Opacity_animation_Completed;
            text_block.BeginAnimation(OpacityProperty, opacity_animation);
        }

        private void Opacity_animation_Completed(object sender, EventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OpenAnimation();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            button_close.IsEnabled = false;
            CloseAnimation();
        }
    }
}
