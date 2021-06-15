using ChessGame.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChessGame.View
{
    /// <summary>
    /// Логика взаимодействия для LoginRegistrationWindow.xaml
    /// </summary>
    public partial class LoginRegistrationWindow : Window
    {
        private double _warning_animation_width;
        private readonly SolidColorBrush _error_brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#de0000"));
        private readonly SolidColorBrush _success_brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#86c93a"));
        private readonly ChessViewModel _chess_vm;
        public LoginRegistrationWindow()
        {
            InitializeComponent();
        }

        public LoginRegistrationWindow(ChessViewModel chess_vm) : this()
        {
            if (chess_vm is null) Close();
            chess_vm.LoginVerified += LoginVerified;
            chess_vm.RegistrationVerified += RegistrationVerified;
            _chess_vm = chess_vm;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _warning_animation_width = border_notify.ActualWidth;
        }

        private void LoginVerified(object sender, bool login_verified, string message)
        {
            if (login_verified)
            {
                ShowNotify(_success_brush, message);
                CloseWindowAnimation();
            }
            else ShowNotify(_error_brush, message);
        }

        private void RegistrationVerified(object sender, bool registration_verigied, string message)
        {
            ShowNotify(registration_verigied ? _success_brush : _error_brush, message);
        }

        private void PasswordBoxPasswordChanged(object sender, RoutedEventArgs e)
        {
            text_block_password_mark.Visibility = password_box.Password.Length > 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        private void ButtonLoginClick(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(text_box_login.Text.Trim()) || string.IsNullOrEmpty(password_box.Password.Trim()))
            {
                if(border_notify.Visibility != Visibility.Visible)
                {
                    ShowNotify(_error_brush, "Введите логин и пароль");
                }
            }
            else if (text_box_login.Text.Trim().Length < 4 || password_box.Password.Trim().Length < 4)
            {
                if (border_notify.Visibility != Visibility.Visible)
                {
                    ShowNotify(_error_brush, "Длина логина и пароля должна быть не менее 4 символов.");
                }
            }
            else _chess_vm.LoginCommand.Execute(new Tuple<object, object>(text_box_login.Text.Trim(), password_box.Password.Trim()));
        }

        private void ButtonRegistrationClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(text_box_login.Text.Trim()) || string.IsNullOrEmpty(password_box.Password.Trim()))
            {
                if (border_notify.Visibility != Visibility.Visible)
                {
                    ShowNotify(_error_brush, "Введите логин и пароль");
                }
            }
            else if (text_box_login.Text.Trim().Length < 4 || password_box.Password.Trim().Length < 4)
            {
                if (border_notify.Visibility != Visibility.Visible)
                {
                    ShowNotify(_error_brush, "Длина логина и пароля должна быть не менее 4 символов.");
                }
            }
            else _chess_vm.RegistrationCommand.Execute(new Tuple<object, object>(text_box_login.Text.Trim(), password_box.Password.Trim()));
        }

        private void KeyboardFocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(border_notify.Visibility == Visibility.Visible) HideNotify();
        }

        private void ShowNotify(SolidColorBrush notify_brush, string message)
        {
            label_notify.Content = message;
            border_notify.Visibility = Visibility.Visible;
            label_notify.Visibility = Visibility.Collapsed;
            border_login.BorderThickness = new Thickness(3);
            border_login.BorderBrush = notify_brush;
            border_password.BorderThickness = new Thickness(3);
            border_password.BorderBrush = notify_brush;
            border_notify.Background = notify_brush;
            DoubleAnimation show_notify_width_animation = new DoubleAnimation(0, _warning_animation_width, new Duration(TimeSpan.FromSeconds(0.2)));
            show_notify_width_animation.Completed += ShowNotifyWidthAnimationCompleted;
            border_notify.BeginAnimation(WidthProperty, show_notify_width_animation);
        }

        private void ShowNotifyWidthAnimationCompleted(object sender, EventArgs e)
        {
            label_notify.Visibility = Visibility.Visible;
        }

        private void HideNotify()
        {
            border_login.BorderThickness = new Thickness(0);
            border_password.BorderThickness = new Thickness(0);
            label_notify.Visibility = Visibility.Collapsed;
            DoubleAnimation close_notify_width_animation = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.2)));
            close_notify_width_animation.Completed += CloseNotifyWidthAnimationCompleted;
            border_notify.BeginAnimation(WidthProperty, close_notify_width_animation);
        }

        private void CloseWindowAnimation()
        {
            DoubleAnimation opacity_animation = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(2)));
            opacity_animation.Completed += OpacityAnimationCompleted;
            BeginAnimation(OpacityProperty, opacity_animation);
        }

        private void OpacityAnimationCompleted(object sender, EventArgs e)
        {
            Close();
        }

        private void CloseNotifyWidthAnimationCompleted(object sender, EventArgs e)
        {
            border_notify.Visibility = Visibility.Collapsed;
        }
    }
}
