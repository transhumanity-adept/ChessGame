using ChessGame.ViewModel;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ChessGame
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SolidColorBrush _error_brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#de0000"));
        private readonly SolidColorBrush _success_brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#86c93a"));
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                Left = 0;
                Top = 0;
            }
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ChessViewModel)) return;
            ((ChessViewModel)DataContext).DataWorkCompleted += ChessVMDataWorkCompleted;
        }

        private void ChessVMDataWorkCompleted(object sender, bool result, string message)
        {
            if (result) ShowNotify(_success_brush, message);
            else ShowNotify(_error_brush, message);
        }

        private void ShowNotify(SolidColorBrush notify_brush, string message)
        {
            _lable_notify.Content = message;
            _border_notify.Visibility = Visibility.Visible;
            _lable_notify.Visibility = Visibility.Collapsed;
            _border_notify.Background = notify_brush;
            DoubleAnimation show_notify_animation = new DoubleAnimation(0, _grid_game_board.ActualWidth - 20, new Duration(TimeSpan.FromSeconds(0.3)));
            show_notify_animation.Completed += ShowNotifyAnimationCompleted;
            _border_notify.BeginAnimation(WidthProperty, show_notify_animation);
        }

        private async void ShowNotifyAnimationCompleted(object sender, EventArgs e)
        {
            _lable_notify.Visibility = Visibility.Visible;
            await Task.Delay(2000);
            HideNotify();
        }

        private void HideNotify()
        {
            _lable_notify.Visibility = Visibility.Collapsed;
            DoubleAnimation close_notify_animation = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.3)));
            close_notify_animation.Completed += HideNotifyAnimationCompleted;
            _border_notify.BeginAnimation(WidthProperty, close_notify_animation);
        }

        private void HideNotifyAnimationCompleted(object sender, EventArgs e)
        {
            _border_notify.Visibility = Visibility.Collapsed;
        }

        private void ButtonOptionsClick(object sender, RoutedEventArgs e)
        {
            if (border_options.Visibility == Visibility.Collapsed) ShowOptions();
            else HideOptions();
        }

        private void ShowOptions()
        {
            if(BorderUploadGame.Visibility != Visibility.Collapsed)
            {
                ListBoxUploadedGames.Visibility = Visibility.Collapsed;
                ButtonUploadGame.Visibility = Visibility.Collapsed;
                DoubleAnimation hide_upload_animation = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.6)));
                hide_upload_animation.Completed += HideUploadComplited;
                BorderUploadGame.BeginAnimation(WidthProperty, hide_upload_animation);
            }
            else
            {
                border_options.Visibility = Visibility.Visible;
                DoubleAnimation width_animation = new DoubleAnimation(0, 200, new Duration(TimeSpan.FromSeconds(0.6)));
                width_animation.Completed += OptionsOpenAnimationCompleted;
                border_options.BeginAnimation(WidthProperty, width_animation);
            }
        }

        private void HideUploadComplited(object sender, EventArgs e)
        {
            BorderUploadGame.Visibility = Visibility.Collapsed;
            border_options.Visibility = Visibility.Visible;
            DoubleAnimation width_animation = new DoubleAnimation(0, 200, new Duration(TimeSpan.FromSeconds(0.6)));
            width_animation.Completed += OptionsOpenAnimationCompleted;
            border_options.BeginAnimation(WidthProperty, width_animation);
        }

        private void OptionsOpenAnimationCompleted(object sender, EventArgs e)
        {
            button_options_flip_sides.Visibility = Visibility.Visible;
            grid_options_black_time.Visibility = Visibility.Visible;
            grid_options_white_time.Visibility = Visibility.Visible;
        }

        private void HideOptions()
        {
            button_options_flip_sides.Visibility = Visibility.Collapsed;
            grid_options_black_time.Visibility = Visibility.Collapsed;
            grid_options_white_time.Visibility = Visibility.Collapsed;
            DoubleAnimation width_animation = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.6)));
            width_animation.Completed += OptionsHideAnimationCompleted;
            border_options.BeginAnimation(WidthProperty, width_animation);
        }

        private void OptionsHideAnimationCompleted(object sender, EventArgs e)
        {
            border_options.Visibility = Visibility.Collapsed;
        }

        private void ButtonHideMainMenuClick(object sender, RoutedEventArgs e)
        {
            HideMainMenu();
        }
        private void ButtonShowMainMenuClick(object sender, RoutedEventArgs e)
        {
            ShowMainMenu();
        }

        private void ShowMainMenu()
        {
            border_main_menu.Visibility = Visibility.Visible;
            grid_show_main_menu.Visibility = Visibility.Collapsed;
            DoubleAnimation width_animation = new DoubleAnimation(0, 190, new Duration(TimeSpan.FromSeconds(0.6)));
            width_animation.Completed += ShowMainMenuAnimationCompleted;
            border_main_menu.BeginAnimation(WidthProperty, width_animation);
        }

        private void ShowMainMenuAnimationCompleted(object sender, EventArgs e)
        {
            grid_main_menu_buttons.Visibility = Visibility.Visible;
        }

        private void HideMainMenu()
        {
            if(border_options.Visibility != Visibility.Collapsed || BorderUploadGame.Visibility != Visibility.Collapsed)
            {
                if(border_options.Visibility != Visibility.Collapsed)
                {
                    button_options_flip_sides.Visibility = Visibility.Collapsed;
                    grid_options_black_time.Visibility = Visibility.Collapsed;
                    grid_options_white_time.Visibility = Visibility.Collapsed;
                    DoubleAnimation width_animation = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.6)));
                    width_animation.Completed += HideMainMenuPartOptionsAnimationCompleted;
                    border_options.BeginAnimation(WidthProperty, width_animation);
                }
                else
                {
                    ListBoxUploadedGames.Visibility = Visibility.Collapsed;
                    ButtonUploadGame.Visibility = Visibility.Collapsed;
                    DoubleAnimation hide_upload_animation = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.6)));
                    hide_upload_animation.Completed += HideMainMenuPartUploadAnimationCompleted;
                    BorderUploadGame.BeginAnimation(WidthProperty, hide_upload_animation);
                }
            }
            else
            {
                grid_main_menu_buttons.Visibility = Visibility.Collapsed;
                DoubleAnimation width_animation = new DoubleAnimation(190, 0, new Duration(TimeSpan.FromSeconds(0.6)));
                width_animation.Completed += HideMainMenuAnimationCompleted;
                border_main_menu.BeginAnimation(WidthProperty, width_animation);
            }
        }

        private void HideMainMenuPartUploadAnimationCompleted(object sender, EventArgs e)
        {
            BorderUploadGame.Visibility = Visibility.Collapsed;
            grid_main_menu_buttons.Visibility = Visibility.Collapsed;
            DoubleAnimation width_animation = new DoubleAnimation(190, 0, new Duration(TimeSpan.FromSeconds(0.6)));
            width_animation.Completed += HideMainMenuAnimationCompleted;
            border_main_menu.BeginAnimation(WidthProperty, width_animation);
        }

        private void HideMainMenuPartOptionsAnimationCompleted(object sender, EventArgs e)
        {
            border_options.Visibility = Visibility.Collapsed;
            grid_main_menu_buttons.Visibility = Visibility.Collapsed;
            DoubleAnimation width_animation = new DoubleAnimation(190, 0, new Duration(TimeSpan.FromSeconds(0.6)));
            width_animation.Completed += HideMainMenuAnimationCompleted;
            border_main_menu.BeginAnimation(WidthProperty, width_animation);
        }

        private void HideMainMenuAnimationCompleted(object sender, EventArgs e)
        {
            grid_show_main_menu.Visibility = Visibility;
        }

        private void ButtonNewGameClick(object sender, RoutedEventArgs e)
        {
            if (border_options.Visibility != Visibility.Collapsed) HideOptions();
            if (BorderUploadGame.Visibility != Visibility.Collapsed) HideUpload();
            DoubleAnimation show_game_board = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(1.5)));
            ItemsContolBoard.BeginAnimation(OpacityProperty, show_game_board);
        }

        private void ButtonUploapClick(object sender, RoutedEventArgs e)
        {
            if (BorderUploadGame.Visibility != Visibility.Visible)
            {
                ((ChessViewModel)DataContext).UploadGamesInformation.Execute(null);
                ShowUpload();
            }
            else HideUpload();
        }

        private void HideUpload()
        {
            ListBoxUploadedGames.Visibility = Visibility.Collapsed;
            ButtonUploadGame.Visibility = Visibility.Collapsed;
            DoubleAnimation hide_upload_animation = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.6)));
            hide_upload_animation.Completed += HideUploadAnimationCompleted;
            BorderUploadGame.BeginAnimation(WidthProperty, hide_upload_animation);
        }

        private void ShowUpload()
        {
            if(border_options.Visibility != Visibility.Collapsed)
            {
                button_options_flip_sides.Visibility = Visibility.Collapsed;
                grid_options_black_time.Visibility = Visibility.Collapsed;
                grid_options_white_time.Visibility = Visibility.Collapsed;
                DoubleAnimation width_animation = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.6)));
                width_animation.Completed += OptionsHideCompited;
                border_options.BeginAnimation(WidthProperty, width_animation);
            }
            else
            {
                BorderUploadGame.Visibility = Visibility.Visible;
                DoubleAnimation show_upload_animation = new DoubleAnimation(0, 250, new Duration(TimeSpan.FromSeconds(0.6)));
                show_upload_animation.Completed += ShowUploadAnimationCompleted;
                BorderUploadGame.BeginAnimation(WidthProperty, show_upload_animation);
            }
        }

        private void ShowUploadAnimationCompleted(object sender, EventArgs e)
        {
            ListBoxUploadedGames.Visibility = Visibility.Visible;
            ButtonUploadGame.Visibility = Visibility.Visible;
        }

        private void OptionsHideCompited(object sender, EventArgs e)
        {
            border_options.Visibility = Visibility.Collapsed;
            BorderUploadGame.Visibility = Visibility.Visible;
            DoubleAnimation show_upload_animation = new DoubleAnimation(0, 250, new Duration(TimeSpan.FromSeconds(0.6)));
            show_upload_animation.Completed += ShowUploadAnimationCompleted;
            BorderUploadGame.BeginAnimation(WidthProperty, show_upload_animation);
        }

        private void HideUploadAnimationCompleted(object sender, EventArgs e)
        {
            BorderUploadGame.Visibility = Visibility.Collapsed;
        }

        private void ButtonUploadGameClick(object sender, RoutedEventArgs e)
        {
            if (ListBoxUploadedGames.SelectedItem is null) return;
            HideUpload();
            ((ChessViewModel)DataContext).UploadGame.Execute(ListBoxUploadedGames.SelectedItem.ToString());
            DoubleAnimation show_game_board = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(1.5)));
            ItemsContolBoard.BeginAnimation(OpacityProperty, show_game_board);
        }
    }
}
