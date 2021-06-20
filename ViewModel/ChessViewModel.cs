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
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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
        private ICommand _save_game_command;
        private ICommand _new_game_command;
        private ICommand _upload_game;
        private ICommand _upload_games_information;
        private ICommand _give_up_command;
        private ICommand _end_graw_command;
        private ICommand _flip_sides_command;
        private Button _current_clicked_button;
        #endregion

        #region События
        public delegate void ClickedCellHandled(object sender, CellClickedEventArgs e);
        public event ClickedCellHandled ClickedOnCell;
        public delegate void ResultOfPawnChangeObtainedHandler(object sender, ResultOfPawnChangeObtainedEventArgs e);
        public event ResultOfPawnChangeObtainedHandler ResultOfPawnChangeObtained;
        public event Action<object, bool, string> LoginVerified;
        public event Action<object, bool, string> RegistrationVerified;
        public event Action<object, bool, string> DataWorkCompleted;
        public event Action GameSaved;
        public event Action<GameResult> GameIsOver;
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
            set
            {
                _white_player = value;
                OnPropertyChanged();
            }
        }
        public Player BlackPlayer
        {
            get => _black_player;
            set
            {
                _black_player = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<string> Dates { get; set; } = new ObservableCollection<string>();
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
                catch
                {
                    RegistrationVerified?.Invoke(this, false, "Ошибка при работа с базой данных, попробуйте позже.");
                }
            }
        }));
        public ICommand SaveGameCommand => _save_game_command ?? (_save_game_command = new RelayCommand(obj => 
        {
            Task.Run(() =>
            {
                string game_state = Game.ToString();
                using (SqlConnection connection = new SqlConnection(RelativePaths.DataBaseConnection))
                {
                    try
                    {
                        connection.Open();
                        if (connection.State != ConnectionState.Open)
                        {
                            App.Current.Dispatcher.Invoke(() => DataWorkCompleted?.Invoke(this, false, "Подключение к базе данных не удалось, попробуйте позже."));
                        }
                        else
                        {
                            if(Game.SaveDate == DateTime.MinValue)
                            {
                                SqlCommand save_command = new SqlCommand($@"INSERT INTO Games([LoginWhitePlayer], [LoginBlackPlayer], [Date], [SerializedData]) "
                                    + $@"VALUES ('{_white_player.Login}','{_black_player.Login}','{DateTime.Now:yyyy-MM-dd HH:mm:ss}','{game_state}')", connection);
                                if(save_command.ExecuteNonQuery() > 0) 
                                    App.Current.Dispatcher.Invoke(() => { DataWorkCompleted?.Invoke(this, true, "Игра сохранена."); GameSaved?.Invoke();});
                                else
                                    App.Current.Dispatcher.Invoke(() => { DataWorkCompleted?.Invoke(this, false, "Сохранение не удалось, попробуйте позже."); });
                            }
                            else
                            {
                                SqlCommand updade_command = new SqlCommand($@"UPDATE Games SET [Date] = '{DateTime.Now:yyyy-MM-dd HH:mm:ss}', [SerializedData] = '{game_state}' " 
                                    + $@"WHERE [LoginWhitePlayer] = '{_white_player.Login}' AND [LoginBlackPlayer] = '{_black_player.Login}' AND [Date] = '{Game.SaveDate:yyyy-MM-dd HH:mm:ss}'", connection);
                                if (updade_command.ExecuteNonQuery() > 0)
                                    App.Current.Dispatcher.Invoke(() => { DataWorkCompleted?.Invoke(this, true, "Данные о игре обновлены."); GameSaved?.Invoke(); });
                                else
                                    App.Current.Dispatcher.Invoke(() => { DataWorkCompleted?.Invoke(this, false, "Обновление данных не удалось, попробуйте позже."); });
                            }
                        }
                    }
                    catch
                    {
                        App.Current.Dispatcher.Invoke(() => DataWorkCompleted?.Invoke(this, false, "Ошибка при работа с базой данных, попробуйте позже."));
                    }
                }
            });
        }));
        public ICommand UploadGamesInformation => _upload_games_information ?? (_upload_games_information = new RelayCommand(obj => 
        {
            Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(RelativePaths.DataBaseConnection))
                {
                    try
                    {
                        connection.Open();
                        if (connection.State != ConnectionState.Open)
                        {
                            App.Current.Dispatcher.Invoke(() => DataWorkCompleted?.Invoke(this, false, "Подключение к базе данных не удалось, попробуйте позже."));
                        }
                        else
                        {
                            SqlCommand save_command = new SqlCommand($@"SELECT [Date] FROM Games WHERE [LoginWhitePlayer] = '{_white_player.Login}' AND [LoginBlackPlayer] = '{_black_player.Login}'", connection);
                            using (SqlDataReader reader = save_command.ExecuteReader())
                            {
                                App.Current.Dispatcher.Invoke(() => Dates.Clear());
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        DateTime game_date = reader.GetDateTime(0);
                                        App.Current.Dispatcher.Invoke(() => Dates.Add(game_date.ToString()));
                                    }
                                    App.Current.Dispatcher.Invoke(() => DataWorkCompleted?.Invoke(this, true, "Загрузка успешна. Совместные игры загружены."));
                                }
                                else App.Current.Dispatcher.Invoke(() => DataWorkCompleted?.Invoke(this, true, "Загрузка успешна. Совместных игр не найдено."));
                            }
                        }
                    }
                    catch
                    {
                        App.Current.Dispatcher.Invoke(() => DataWorkCompleted?.Invoke(this, false, "Ошибка при работа с базой данных, попробуйте позже."));
                    }
                }
            });
        }));
        public ICommand UploadGame => _upload_game ?? (_upload_game = new RelayCommand(obj =>
        {
            if (!(obj is string date)) return;
            DateTime game_save_date = DateTime.Parse(date);
            Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(RelativePaths.DataBaseConnection))
                {
                    try
                    {
                        connection.Open();
                        if (connection.State != ConnectionState.Open)
                        {
                            App.Current.Dispatcher.Invoke(() => DataWorkCompleted?.Invoke(this, false, "Подключение к базе данных не удалось, попробуйте позже."));
                        }
                        else
                        {
                            SqlCommand upload_command = new SqlCommand($@"SELECT [SerializedData] FROM Games WHERE [LoginWhitePlayer] = '{_white_player.Login}' AND [LoginBlackPlayer] = '{_black_player.Login}' AND [Date] = '{game_save_date:yyyy-MM-dd HH:mm:ss}'", connection);
                            using (SqlDataReader reader = upload_command.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    reader.Read();
                                    string game_info = reader.GetString(0);
                                    App.Current.Dispatcher.Invoke(() => 
                                    {
                                        Game game = new Game(game_save_date, this, game_info);
                                        game.GameOver += GameOver;
                                        game.EventsDetached += GameEventsDetached;
                                        game.Board.PawnChanged += Board_PawnChanged;
                                        Game = game;
                                        DataWorkCompleted?.Invoke(this, true, "Игра загружена");
                                    });
                                }
                                else App.Current.Dispatcher.Invoke(() => DataWorkCompleted?.Invoke(this, false, "Данные о игре не найдены"));
                            }
                        }
                    }
                    catch
                    {
                        App.Current.Dispatcher.Invoke(() => DataWorkCompleted?.Invoke(this, false, "Ошибка при работа с базой данных, попробуйте позже."));
                    }
                }
            });
        }));
        public ICommand NewGameCommand => _new_game_command ?? (_new_game_command = new RelayCommand(obj =>
        {
            var (obj_one, obj_two) = obj as Tuple<object, object>;
            if (!(obj_one is double white_seconds) || !(obj_two is double black_seconds)) return;
            Game game = new Game(DateTime.MinValue, this, (int)white_seconds, (int)black_seconds);
            game.GameOver += GameOver;
            game.EventsDetached += GameEventsDetached;
            game.Board.PawnChanged += Board_PawnChanged;
            Game = game;
        }));
        public ICommand GiveUpCommand => _give_up_command ?? (_give_up_command = new RelayCommand(obj => 
        {
            Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(RelativePaths.DataBaseConnection))
                {
                    try
                    {
                        connection.Open();
                        if (connection.State != ConnectionState.Open)
                        {
                            App.Current.Dispatcher.Invoke(() => DataWorkCompleted?.Invoke(this, false, "Подключение к базе данных не удалось, попробуйте позже."));
                        }
                        else
                        {
                            Player losed_player;
                            Player winning_player;
                            GameResult game_result;
                            if(Game.Board.CurrentMoveColor == FigureColor.White) 
                            { 
                                losed_player = _white_player; 
                                winning_player = _black_player;
                                game_result = GameResult.BlackWin; 
                            }
                            else 
                            { 
                                losed_player = _black_player; 
                                winning_player = _white_player;
                                game_result = GameResult.WhiteWin;
                            }
                            SqlCommand update_losed_player_info_command = new SqlCommand($@"UPDATE Players SET [Rating] = '{losed_player.Rating - 25}' WHERE [Login] = '{losed_player.Login}'", connection);
                            if (update_losed_player_info_command.ExecuteNonQuery() == 0)
                            {
                                App.Current.Dispatcher.Invoke(() => DataWorkCompleted?.Invoke(this, false, "Обновление рейтинга не удалось."));
                                return;
                            }
                            SqlCommand update_winning_player_info_command = new SqlCommand($@"UPDATE Players SET [Rating] = '{winning_player.Rating + 25}' WHERE [Login] = '{winning_player.Login}'", connection);
                            if (update_winning_player_info_command.ExecuteNonQuery() > 0)
                            {
                                App.Current.Dispatcher.Invoke(() => 
                                {
                                    losed_player.Rating -= 25;
                                    winning_player.Rating += 25;
                                    GameIsOver?.Invoke(game_result); 
                                });
                            }
                            else
                                App.Current.Dispatcher.Invoke(() => DataWorkCompleted?.Invoke(this, false, "Обновление рейтинга не удалось."));
                        }
                    }
                    catch (Exception e)
                    {
                        App.Current.Dispatcher.Invoke(() => DataWorkCompleted?.Invoke(this, false, "Ошибка при работа с базой данных, попробуйте позже."));
                    }
                }
            });
        }));
        public ICommand EndDrawCommand => _end_graw_command ?? (_end_graw_command = new RelayCommand(obj => 
        {
            GameIsOver?.Invoke(GameResult.Draw);
        }));
        public ICommand FlipSidesCommand => _flip_sides_command ?? (_flip_sides_command = new RelayCommand(obj => 
        {
            Player tmp = WhitePlayer;
            WhitePlayer = BlackPlayer;
            BlackPlayer = tmp;
        }));
        #endregion
        #region Конструкторы
        public ChessViewModel()
        {
            Window window = new LoginRegistrationWindow(this);
            window.ShowDialog();
            window = new LoginRegistrationWindow(this);
            window.ShowDialog();
            if (_white_player is null || _black_player is null) App.Current.MainWindow.Close();
        }
        #endregion

        private void GameEventsDetached()
        {
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (Game == null) return;
                Game.GameOver -= GameOver;
                Game.EventsDetached -= GameEventsDetached;
                Game.Board.PawnChanged -= Board_PawnChanged;
                Game = null;
            }));
        }
        private void GameOver(object sender, GameOverEventArgs e)
        {
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (Game == null) return;
                if (Game.SaveDate != DateTime.MinValue) RemoveGameFromDataBase(Game.SaveDate, _white_player.Login, _black_player.Login);
                Game.GameOver -= GameOver;
                Game.Board.PawnChanged -= Board_PawnChanged;
                Game = null;
                var main_window = App.Current.MainWindow;
                GameResultWindow game_result = new GameResultWindow(e.GameResult);
                game_result.ShowDialog();
            }));
        }
        private void RemoveGameFromDataBase(DateTime save_date, string white_player_login, string black_player_login)
        {
            Task.Run(() =>
            {
                using (SqlConnection connection = new SqlConnection(RelativePaths.DataBaseConnection))
                {
                    try
                    {
                        connection.Open();
                        if (connection.State != ConnectionState.Open)
                        {
                            App.Current.Dispatcher.Invoke(() => DataWorkCompleted?.Invoke(this, false, "Подключение к базе данных не удалось, попробуйте позже."));
                        }
                        else
                        {
                            SqlCommand remove_command = new SqlCommand($@"DELETE FROM Games WHERE [LoginWhitePlayer] = '{white_player_login}' AND [LoginBlackPlayer] = '{black_player_login}' AND [Date] = '{save_date:yyyy-MM-dd HH:mm:ss}'", connection);
                            if (remove_command.ExecuteNonQuery() > 0)
                                App.Current.Dispatcher.Invoke(() => { DataWorkCompleted?.Invoke(this, true, "Данные о игре обновлены."); GameSaved?.Invoke(); });
                            else
                                App.Current.Dispatcher.Invoke(() => { DataWorkCompleted?.Invoke(this, false, "Обновление данных не удалось, попробуйте позже."); });
                        }
                    }
                    catch
                    {
                        App.Current.Dispatcher.Invoke(() => DataWorkCompleted?.Invoke(this, false, "Ошибка при работа с базой данных, попробуйте позже."));
                    }
                }
            });
        }

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
