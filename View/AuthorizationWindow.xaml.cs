using ChessGame.Model;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Linq;

namespace ChessGame.View
{
    /// <summary>
    /// Логика взаимодействия для AuthenticationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        #region Поля
        private double _warning_animation_width;
        private readonly SolidColorBrush _error_brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#de0000"));
        private readonly SolidColorBrush _success_brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#86c93a"));
        private readonly Game _chess_game;
        #endregion

        #region Конструкторы
        public AuthorizationWindow()
        {
            InitializeComponent();
        }
        public AuthorizationWindow(Game chess_vm) : this()
        {
            if (chess_vm is null) Close();
            chess_vm.LoginVerified += LoginVerified;
            chess_vm.RegistrationVerified += RegistrationVerified;
            _chess_game = chess_vm;
        }
        #endregion

        #region Методы
        /// <summary>
        /// Обработчик события "Окно загружено"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            _warning_animation_width = _border_notify.ActualWidth;
        }
        /// <summary>
        /// Обработчик события "Логин верифицирован"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="login_verified">Состояние верификации</param>
        /// <param name="message">Сообщение</param>
        private void LoginVerified(object sender, bool login_verified, string message)
        {
            if (login_verified)
            {
                ShowNotify(_success_brush, message);
                CloseWindowAnimation();
            }
            else ShowNotify(_error_brush, message);
        }
        /// <summary>
        /// Обработчик события "Регистрация верифицирована"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="registration_verigied">Состояние верификации</param>
        /// <param name="message">Сообщение</param>
        private void RegistrationVerified(object sender, bool registration_verigied, string message)
        {
            ShowNotify(registration_verigied ? _success_brush : _error_brush, message);
        }
        /// <summary>
        /// Обработчик события "Пароль изменился"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void PasswordBoxPasswordChanged(object sender, RoutedEventArgs e)
        {
            _text_block_password_mark.Visibility = _password_box.Password.Length > 0 ? Visibility.Collapsed : Visibility.Visible;
        }
        /// <summary>
        /// Обработчик события "Клик по кнопке "Логин""
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void ButtonLoginClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_text_box_login.Text.Trim()) || string.IsNullOrEmpty(_password_box.Password.Trim()))
            {
                if (_border_notify.Visibility != Visibility.Visible)
                {
                    ShowNotify(_error_brush, "Введите логин и пароль");
                }
            }
            else if (_text_box_login.Text.Trim().Length < 4 || _password_box.Password.Trim().Length < 4)
            {
                if (_border_notify.Visibility != Visibility.Visible)
                {
                    ShowNotify(_error_brush, "Длина логина и пароля должна быть не менее 4 символов.");
                }
            }
            else _chess_game.LoginCommand.Execute(new Tuple<object, object>(_text_box_login.Text.Trim(), _password_box.Password.Trim()));
        }
        /// <summary>
        /// Обработчик события "Клик по кнопке "Регистрация""
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void ButtonRegistrationClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_text_box_login.Text.Trim()) || string.IsNullOrEmpty(_password_box.Password.Trim()))
            {
                if (_border_notify.Visibility != Visibility.Visible)
                {
                    ShowNotify(_error_brush, "Введите логин и пароль");
                }
            }
            else if (_text_box_login.Text.Trim().Length < 4 || _password_box.Password.Trim().Length < 4)
            {
                if (_border_notify.Visibility != Visibility.Visible)
                {
                    ShowNotify(_error_brush, "Длина логина и пароля должна быть не менее 4 символов.");
                }
            }
            else if (_text_box_login.Text.Trim().ToList().Any(c => !char.IsLetterOrDigit(c)) || _password_box.Password.Trim().ToList().Any(c => !char.IsLetterOrDigit(c)))
            {
                if (_border_notify.Visibility != Visibility.Visible)
                {
                    ShowNotify(_error_brush, "Запрещено использование специальных символов.");
                }
            }
            else _chess_game.RegistrationCommand.Execute(new Tuple<object, object>(_text_box_login.Text.Trim(), _password_box.Password.Trim()));
        }
        /// <summary>
        /// Обработчик события "Фокус ввода изменился"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void KeyboardFocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_border_notify.Visibility == Visibility.Visible) HideNotify();
        }
        /// <summary>
        /// Отобразить уведомление
        /// </summary>
        /// <param name="notify_brush">Цвет уведомления</param>
        /// <param name="message">Текст уведомления</param>
        private void ShowNotify(SolidColorBrush notify_brush, string message)
        {
            _label_notify.Content = message;
            _border_notify.Visibility = Visibility.Visible;
            _label_notify.Visibility = Visibility.Collapsed;
            _border_login.BorderThickness = new Thickness(3);
            _border_login.BorderBrush = notify_brush;
            _border_password.BorderThickness = new Thickness(3);
            _border_password.BorderBrush = notify_brush;
            _border_notify.Background = notify_brush;
            DoubleAnimation show_notify_width_animation = new DoubleAnimation(0, _warning_animation_width, new Duration(TimeSpan.FromSeconds(0.2)));
            show_notify_width_animation.Completed += ShowNotifyWidthAnimationCompleted;
            _border_notify.BeginAnimation(WidthProperty, show_notify_width_animation);
        }
        /// <summary>
        /// Обработчик события "Анимация показа уведомления завершена"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void ShowNotifyWidthAnimationCompleted(object sender, EventArgs e)
        {
            _label_notify.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Скрыть уведомление
        /// </summary>
        private void HideNotify()
        {
            _border_login.BorderThickness = new Thickness(0);
            _border_password.BorderThickness = new Thickness(0);
            _label_notify.Visibility = Visibility.Collapsed;
            DoubleAnimation close_notify_width_animation = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.2)));
            close_notify_width_animation.Completed += CloseNotifyWidthAnimationCompleted;
            _border_notify.BeginAnimation(WidthProperty, close_notify_width_animation);
        }
        /// <summary>
        /// Обработчик события "Анимация скрытия уведомления завершена"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void CloseNotifyWidthAnimationCompleted(object sender, EventArgs e)
        {
            _border_notify.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// Анимация закрытия окна
        /// </summary>
        private void CloseWindowAnimation()
        {
            DoubleAnimation opacity_animation = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(2)));
            opacity_animation.Completed += OpacityAnimationCompleted;
            BeginAnimation(OpacityProperty, opacity_animation);
        }
        /// <summary>
        /// Обработчик события "Анимация закрытия формы завершена"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void OpacityAnimationCompleted(object sender, EventArgs e)
        {
            Close();
        }
        #endregion
    }
}
