using System.Windows.Input;
using ChessGame.Model;
using ChessGame.Helpers;
using ChessGame.Model.Figures.Helpers;
using ChessGame.View;
using ChessGame.Model.Helpers;
using ChessGame.ViewModel.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Data;
using System;
using System.IO;

namespace ChessGame.ViewModel
{
    public class ChessViewModel : NotifyPropertyChanged
    {
        #region Поля
        private Game _game;
        private Player _white_player;
        private Player _black_player;
        private ICommand _cell_click_command;
        private ICommand _login_command;
        private ICommand _registration_command;
        private Button _current_clicked_button;
        #endregion

        #region События
        public delegate void ClickedCellHandled(object sender, CellClickedEventArgs e);
        public event ClickedCellHandled ClickedOnCell;
        public delegate void ResultOfPawnChangeObtainedHandler(object sender, ResultOfPawnChangeObtainedEventArgs e);
        public event ResultOfPawnChangeObtainedHandler ResultOfPawnChangeObtained;
        public event Action<object, bool, string> LoginVerified;
        public event Action<object, bool, string> RegistrationVerified;
        #endregion

        #region Свойства
        public Game Game
        {
            get => _game;
            set
            {
                _game = value;
                OnPropertyChanged();
            }
        }

        public Player WhitePlayer
        {
            get => _white_player;
        }

        public Player BlackPlayer
        {
            get => _black_player;
        }

        public ICommand CellClickCommand => _cell_click_command ?? (_cell_click_command = new RelayCommand(obj =>
        {
            var (obj_one, obj_two) = obj as Tuple<object, object>;
            if (!(obj_one is Button button) || !(obj_two is Cell cell)) return;
            _current_clicked_button = button;
            ClickedOnCell?.Invoke(this, new CellClickedEventArgs(cell));
        }));

        public ICommand LoginCommand => _login_command ?? (_login_command = new RelayCommand(obj =>
        {
            var (obj_one, obj_two) = obj as Tuple<object, object>;
            if (!(obj_one is string login) || !(obj_two is string password)) return;
            if(_white_player != null && _white_player.Login == login)
            {
                LoginVerified?.Invoke(this, false, "Данный пользователь уже аутентифицирован.");
                return;
            }
            using (SqlConnection connection = new SqlConnection(RelativePaths.DataBaseConnection))
            {
                try
                {
                    connection.Open();
                    if (connection.State != ConnectionState.Open)
                    {
                        LoginVerified?.Invoke(this, false, "Подключение к базе данных не удалось, попробуйте позже.");
                    }
                    else
                    {
                        SqlCommand select_command = new SqlCommand($"SELECT * FROM Players WHERE Players.[Login] = '{login}'", connection);
                        using (SqlDataReader reader = select_command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                string hash = reader.GetString(1);
                                int rating = reader.GetInt32(2);
                                if (BCrypt.Net.BCrypt.Verify(password, hash))
                                {
                                    if (_white_player == null) _white_player = new Player(rating, login);
                                    else _black_player = new Player(rating, login);
                                    LoginVerified?.Invoke(this, true, "Успешная аутентификация");
                                }
                                else LoginVerified?.Invoke(this, false, "Неверный пароль.");
                            }
                            else LoginVerified?.Invoke(this, false, "Неверный логин.");
                        }
                    }
                }
                catch (Exception e)
                {
                    LoginVerified?.Invoke(this, false, "Ошибка при работа с базой данных, попробуйте позже.");
                }
            }
        }));

        public ICommand RegistrationCommand => _registration_command ?? (_registration_command = new RelayCommand(obj =>
        {
            var (obj_one, obj_two) = obj as Tuple<object, object>;
            if (!(obj_one is string login) || !(obj_two is string password)) return;
            using (SqlConnection connection = new SqlConnection(RelativePaths.DataBaseConnection))
            {
                try
                {
                    connection.Open();
                    if (connection.State != ConnectionState.Open)
                    {
                        RegistrationVerified?.Invoke(this, false, "Подключение к базе данных не удалось, попробуйте позже.");
                    }
                    else
                    {
                        SqlCommand select_command = new SqlCommand($"SELECT * FROM Players WHERE Players.[Login] = '{login}'", connection);
                        using (SqlDataReader reader = select_command.ExecuteReader())
                        {
                            if (reader.HasRows) RegistrationVerified?.Invoke(this, false, "Такой логин уже существует.");
                            else
                            {
                                reader.Close();
                                string hash = BCrypt.Net.BCrypt.HashPassword(password);
                                SqlCommand insert_command = new SqlCommand($"INSERT INTO Players([Login], [Password], [Rating]) VALUES ('{login}', '{hash}', '5000')", connection);
                                int count_row = insert_command.ExecuteNonQuery();
                                if (count_row == 0) throw new Exception();
                                else
                                {
                                    RegistrationVerified?.Invoke(this, true, "Пользователь успешно зарегистрирован.");
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    RegistrationVerified?.Invoke(this, false, "Ошибка при работа с базой данных, попробуйте позже.");
                }
            }
        }));
        #endregion

        #region Конструкторы
        public ChessViewModel()
        {
            Game = new Game(this, 1000, 1000);
            //Window window = new LoginRegistrationWindow(this);
            //window.ShowDialog();
            //window = new LoginRegistrationWindow(this);
            //window.ShowDialog();
            //if (_white_player is null || _black_player is null) App.Current.MainWindow.Close();
            Game.GameOver += GameOver;
            Game.Board.PawnChanged += Board_PawnChanged;
            Game.Board.GameOver += GameOver;
        }

        private void GameOver(object sender, GameOverEventArgs e)
        {
            Game = null;
            GameResultWindow game_result = new GameResultWindow(e.GameResult, App.Current.MainWindow);
            game_result.ShowDialog();
        }
        #endregion

        /// <summary>
        /// Обработка события "Смена пешки"
        /// </summary>
        private void Board_PawnChanged(object sender, PawnChangedEventArgs e)
        {
            PawnChange change_window = new PawnChange(e.Color == FigureColor.White);
            Point relative_location = _current_clicked_button.TranslatePoint(new Point(0, 0), App.Current.MainWindow);
            change_window.Left = relative_location.X + App.Current.MainWindow.Left + 6;
            change_window.Top = e.Color == FigureColor.White ?
                relative_location.Y + App.Current.MainWindow.Top + 30 :
                relative_location.Y + App.Current.MainWindow.Top + 30 - _current_clicked_button.ActualHeight * 3;
            change_window.Width = _current_clicked_button.ActualWidth + 3;
            change_window.Height = _current_clicked_button.ActualHeight * 4;
            change_window.ShowDialog();
            ChangeResult result = change_window.ChangeResult;
            ResultOfPawnChangeObtained?.Invoke(this, new ResultOfPawnChangeObtainedEventArgs(e.Position, e.Color, result));
        }
    }
}
