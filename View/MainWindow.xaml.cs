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
        #region Поля
        private readonly SolidColorBrush _error_brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#de0000"));
        private readonly SolidColorBrush _success_brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#86c93a"));
        #endregion

        #region Конструкторы
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Методы
        /// <summary>
        /// Обработчик события "Размеры окна изменились"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                Left = 0;
                Top = 0;
            }
        }
        /// <summary>
        /// Обработчик события "Окно загружено"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ChessViewModel)) return;
            ((ChessViewModel)DataContext).DataWorkCompleted += ChessVMDataWorkCompleted;
        }
        /// <summary>
        /// Показать главное меню
        /// </summary>
        private void ShowMainMenu()
        {
            border_main_menu.Visibility = Visibility.Visible;
            _grid_show_main_menu.Visibility = Visibility.Collapsed;
            DoubleAnimation width_animation = new DoubleAnimation(0, 190, new Duration(TimeSpan.FromSeconds(0.6)));
            width_animation.Completed += ShowMainMenuAnimationCompleted;
            border_main_menu.BeginAnimation(WidthProperty, width_animation);
        }
        /// <summary>
        /// Обработчик события "Анимация показа меню завершена"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void ShowMainMenuAnimationCompleted(object sender, EventArgs e)
        {
            _grid_main_menu_buttons.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Скрыть главное меню
        /// </summary>
        private void HideMainMenu()
        {
            if (_border_options.Visibility != Visibility.Collapsed || _border_upload_game.Visibility != Visibility.Collapsed)
            {
                if (_border_options.Visibility != Visibility.Collapsed)
                {
                    _button_options_flip_sides.Visibility = Visibility.Collapsed;
                    _grid_options_black_time.Visibility = Visibility.Collapsed;
                    _grid_options_white_time.Visibility = Visibility.Collapsed;
                    DoubleAnimation width_animation = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.6)));
                    width_animation.Completed += HideMainMenuPartOptionsAnimationCompleted;
                    _border_options.BeginAnimation(WidthProperty, width_animation);
                }
                else
                {
                    _list_box_uploaded_games.Visibility = Visibility.Collapsed;
                    _button_upload_game.Visibility = Visibility.Collapsed;
                    DoubleAnimation hide_upload_animation = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.6)));
                    hide_upload_animation.Completed += HideMainMenuPartUploadAnimationCompleted;
                    _border_upload_game.BeginAnimation(WidthProperty, hide_upload_animation);
                }
            }
            else
            {
                _grid_main_menu_buttons.Visibility = Visibility.Collapsed;
                DoubleAnimation width_animation = new DoubleAnimation(190, 0, new Duration(TimeSpan.FromSeconds(0.6)));
                width_animation.Completed += HideMainMenuAnimationCompleted;
                border_main_menu.BeginAnimation(WidthProperty, width_animation);
            }
        }
        /// <summary>
        /// Обработчик события "Часть анимация закрытия главного меню, связанная с загрузками, завершена"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void HideMainMenuPartUploadAnimationCompleted(object sender, EventArgs e)
        {
            _border_upload_game.Visibility = Visibility.Collapsed;
            _grid_main_menu_buttons.Visibility = Visibility.Collapsed;
            DoubleAnimation width_animation = new DoubleAnimation(190, 0, new Duration(TimeSpan.FromSeconds(0.6)));
            width_animation.Completed += HideMainMenuAnimationCompleted;
            border_main_menu.BeginAnimation(WidthProperty, width_animation);
        }
        /// <summary>
        /// Обработчик события "Часть анимация закрытия главного меню, связанная с опциями, завершена"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void HideMainMenuPartOptionsAnimationCompleted(object sender, EventArgs e)
        {
            _border_options.Visibility = Visibility.Collapsed;
            _grid_main_menu_buttons.Visibility = Visibility.Collapsed;
            DoubleAnimation width_animation = new DoubleAnimation(190, 0, new Duration(TimeSpan.FromSeconds(0.6)));
            width_animation.Completed += HideMainMenuAnimationCompleted;
            border_main_menu.BeginAnimation(WidthProperty, width_animation);
        }
        /// <summary>
        /// Обработчик события "Анимация скрытия главного меню завершена"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void HideMainMenuAnimationCompleted(object sender, EventArgs e)
        {
            _grid_show_main_menu.Visibility = Visibility;
        }
        /// <summary>
        /// Обработчик события "Обработка данных завершена"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="result">Результат обработки данных</param>
        /// <param name="message">Сообщение</param>
        private void ChessVMDataWorkCompleted(object sender, bool result, string message)
        {
            if (result) ShowNotify(_success_brush, message);
            else ShowNotify(_error_brush, message);
        }
        /// <summary>
        /// Показать уведомление
        /// </summary>
        /// <param name="notify_brush">Цвет уведомления</param>
        /// <param name="message">Текст уведомления</param>
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
        /// <summary>
        /// Обработчик события "Анимация показа уведомления завершена"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private async void ShowNotifyAnimationCompleted(object sender, EventArgs e)
        {
            _lable_notify.Visibility = Visibility.Visible;
            await Task.Delay(2000);
            HideNotify();
        }
        /// <summary>
        /// Скрыть уведомление
        /// </summary>
        private void HideNotify()
        {
            _lable_notify.Visibility = Visibility.Collapsed;
            DoubleAnimation close_notify_animation = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.3)));
            close_notify_animation.Completed += HideNotifyAnimationCompleted;
            _border_notify.BeginAnimation(WidthProperty, close_notify_animation);
        }
        /// <summary>
        /// Обработчик события "Анимация скрытия уведомления завершена"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HideNotifyAnimationCompleted(object sender, EventArgs e)
        {
            _border_notify.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// Обработчик события "Клик по кнопке "Опции""
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void ButtonOptionsClick(object sender, RoutedEventArgs e)
        {
            if (_border_options.Visibility == Visibility.Collapsed) ShowOptions();
            else HideOptions();
        }
        /// <summary>
        /// Показать раздел с опциями
        /// </summary>
        private void ShowOptions()
        {
            if(_border_upload_game.Visibility != Visibility.Collapsed)
            {
                _list_box_uploaded_games.Visibility = Visibility.Collapsed;
                _button_upload_game.Visibility = Visibility.Collapsed;
                DoubleAnimation hide_upload_animation = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.6)));
                hide_upload_animation.Completed += ShowOptionsPartUploadAnimationCompleted;
                _border_upload_game.BeginAnimation(WidthProperty, hide_upload_animation);
            }
            else
            {
                _border_options.Visibility = Visibility.Visible;
                DoubleAnimation width_animation = new DoubleAnimation(0, 200, new Duration(TimeSpan.FromSeconds(0.6)));
                width_animation.Completed += OptionsOpenAnimationCompleted;
                _border_options.BeginAnimation(WidthProperty, width_animation);
            }
        }
        /// <summary>
        /// Обработчик события "Анимация показа раздела с опициями завершена"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void OptionsOpenAnimationCompleted(object sender, EventArgs e)
        {
            _button_options_flip_sides.Visibility = Visibility.Visible;
            _grid_options_black_time.Visibility = Visibility.Visible;
            _grid_options_white_time.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Скрыть раздел с опциями
        /// </summary>
        private void HideOptions()
        {
            _button_options_flip_sides.Visibility = Visibility.Collapsed;
            _grid_options_black_time.Visibility = Visibility.Collapsed;
            _grid_options_white_time.Visibility = Visibility.Collapsed;
            DoubleAnimation width_animation = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.6)));
            width_animation.Completed += OptionsHideAnimationCompleted;
            _border_options.BeginAnimation(WidthProperty, width_animation);
        }
        /// <summary>
        /// Обработчик события "Анимация скрытия раздела с опциями завершена"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void OptionsHideAnimationCompleted(object sender, EventArgs e)
        {
            _border_options.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// Обработчик события "Часть анимация показа раздела с опциями, связанная с загрузками, завершена"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void ShowOptionsPartUploadAnimationCompleted(object sender, EventArgs e)
        {
            _border_upload_game.Visibility = Visibility.Collapsed;
            _border_options.Visibility = Visibility.Visible;
            DoubleAnimation width_animation = new DoubleAnimation(0, 200, new Duration(TimeSpan.FromSeconds(0.6)));
            width_animation.Completed += OptionsOpenAnimationCompleted;
            _border_options.BeginAnimation(WidthProperty, width_animation);
        }
        /// <summary>
        /// Обработчик события "Клик по кнопке "Скрыть главное меню""
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void ButtonHideMainMenuClick(object sender, RoutedEventArgs e)
        {
            HideMainMenu();
        }
        /// <summary>
        /// Обработчик события "Клик по кнопке "Показать главное меню""
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void ButtonShowMainMenuClick(object sender, RoutedEventArgs e)
        {
            ShowMainMenu();
        }
        /// <summary>
        /// Обработчик события "Клик по кнопке "Новая игра""
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void ButtonNewGameClick(object sender, RoutedEventArgs e)
        {
            if (_border_options.Visibility != Visibility.Collapsed) HideOptions();
            if (_border_upload_game.Visibility != Visibility.Collapsed) HideUpload();
            DoubleAnimation show_game_board = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(1.5)));
            _items_control_board.BeginAnimation(OpacityProperty, show_game_board);
        }
        /// <summary>
        /// Обработчик события "Клик по кнопке "Загрузить игру""
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void ButtonUploapClick(object sender, RoutedEventArgs e)
        {
            if (_border_upload_game.Visibility != Visibility.Visible)
            {
                ((ChessViewModel)DataContext).UploadGamesInformation.Execute(null);
                ShowUpload();
            }
            else HideUpload();
        }
        /// <summary>
        /// Скрыть раздел с загрузками
        /// </summary>
        private void HideUpload()
        {
            _list_box_uploaded_games.Visibility = Visibility.Collapsed;
            _button_upload_game.Visibility = Visibility.Collapsed;
            DoubleAnimation hide_upload_animation = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.6)));
            hide_upload_animation.Completed += HideUploadAnimationCompleted;
            _border_upload_game.BeginAnimation(WidthProperty, hide_upload_animation);
        }
        /// <summary>
        /// Показать раздел с загрузками
        /// </summary>
        private void ShowUpload()
        {
            if(_border_options.Visibility != Visibility.Collapsed)
            {
                _button_options_flip_sides.Visibility = Visibility.Collapsed;
                _grid_options_black_time.Visibility = Visibility.Collapsed;
                _grid_options_white_time.Visibility = Visibility.Collapsed;
                DoubleAnimation width_animation = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.6)));
                width_animation.Completed += ShowUploadPartOptionsAnimationCompleted;
                _border_options.BeginAnimation(WidthProperty, width_animation);
            }
            else
            {
                _border_upload_game.Visibility = Visibility.Visible;
                DoubleAnimation show_upload_animation = new DoubleAnimation(0, 250, new Duration(TimeSpan.FromSeconds(0.6)));
                show_upload_animation.Completed += ShowUploadAnimationCompleted;
                _border_upload_game.BeginAnimation(WidthProperty, show_upload_animation);
            }
        }
        /// <summary>
        /// Обработчик события "Часть анимация закрытия главного меню, связанная с опциями, завершена"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void ShowUploadAnimationCompleted(object sender, EventArgs e)
        {
            _list_box_uploaded_games.Visibility = Visibility.Visible;
            _button_upload_game.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Обработчик события "Часть анимация показа раздела загрузок, связанная с опциями, завершена"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void ShowUploadPartOptionsAnimationCompleted(object sender, EventArgs e)
        {
            _border_options.Visibility = Visibility.Collapsed;
            _border_upload_game.Visibility = Visibility.Visible;
            DoubleAnimation show_upload_animation = new DoubleAnimation(0, 250, new Duration(TimeSpan.FromSeconds(0.6)));
            show_upload_animation.Completed += ShowUploadAnimationCompleted;
            _border_upload_game.BeginAnimation(WidthProperty, show_upload_animation);
        }
        /// <summary>
        /// Обработчик события "Анимация скрытия раздела с загрузками завершена"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void HideUploadAnimationCompleted(object sender, EventArgs e)
        {
            _border_upload_game.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// Обработчик события "Клик по кнопке "Загрузить игру""
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void ButtonUploadGameClick(object sender, RoutedEventArgs e)
        {
            if (_list_box_uploaded_games.SelectedItem is null) return;
            HideUpload();
            ((ChessViewModel)DataContext).UploadGame.Execute(_list_box_uploaded_games.SelectedItem.ToString());
            DoubleAnimation show_game_board = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(1.5)));
            _items_control_board.BeginAnimation(OpacityProperty, show_game_board);
        }
        #endregion
    }
}
