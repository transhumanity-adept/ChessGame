using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using ChessGame.Helpers;
using ChessGame.Model.Figures;
using ChessGame.Model.Figures.Helpers;
using ChessGame.ViewModel;
using ChessGame.Model.Helpers;
using System.Threading.Tasks;

namespace ChessGame.Model
{
    public class Board : NotifyPropertyChanged, IEnumerable<Cell>
    {
        #region Поля
        private readonly Cell[,] _cells = new Cell[8, 8];
        private readonly Figure[,] _figures = new Figure[4, 8];
        private FigureColor _current_move_color;
        private readonly ChessViewModel _chess_vm;
        private readonly Game _game;
        #endregion

        #region Конструкторы
        public Board()
        {
            BoardStartSetup();
            _figures.Cast<Figure>().ToList().ForEach(e => { if (e != null) { e.Moved += FigureMoved; e.Attacked += FigureAttacked; } });
        }
        public Board(Game game, ChessViewModel view_model) : this()
        {
            game.GameOver += GameGameOver;
            view_model.GameSaved += VMGameSaved;
            view_model.GameOver += VMGameOver;
            view_model.ClickedOnCell += ClickedOnCell;
            view_model.ResultOfPawnChangeObtained += ResultOfPawnChangeObtained;
            _game = game;
            _chess_vm = view_model;
        }
        public Board(Game game, ChessViewModel view_model, string restore_state_info)
        {
            game.GameOver += GameGameOver;
            view_model.GameSaved += VMGameSaved;
            view_model.GameOver += VMGameOver;
            view_model.ClickedOnCell += ClickedOnCell;
            view_model.ResultOfPawnChangeObtained += ResultOfPawnChangeObtained;
            _game = game;
            _chess_vm = view_model;
            RestoreState(restore_state_info);
        }
        #endregion

        #region Свойства
        public int CountMoves { get; private set; } = 0;
        public FigureColor CurrentMoveColor
        {
            get => _current_move_color;
            private set
            {
                _current_move_color = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region индексаторы
        public Cell this[int row, int column]
        {
            get => _cells[row, column];
            set => _cells[row, column] = value;
        }
        #endregion

        #region События
        /// <summary>
        /// Событие "Получен результат изменения пешки"
        /// </summary>
        public event Action<object, Position, FigureColor> PawnChangedResult;
        /// <summary>
        /// Событие "Игра окончена"
        /// </summary>
        public event Action<object, GameResult> GameOver;
        /// <summary>
        /// Событие "Подписки на события обнулены"
        /// </summary>
        public event Action<object> EventsDetached;
        #endregion

        #region Методы
        /// <summary>
        /// Обработчик события "Игра окончена", источник Game
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="game_result">Результат игры</param>
        private void GameGameOver(object sender, GameResult game_result)
        {
            CheckmateCache.Clear();
            _chess_vm.ClickedOnCell -= ClickedOnCell;
            _chess_vm.ResultOfPawnChangeObtained -= ResultOfPawnChangeObtained;
            _game.GameOver -= GameGameOver;
        }
        /// <summary>
        /// Обработчик события "Игра окончена", источник VM
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="game_result">Результат игры</param>
        private void VMGameOver(object sender, GameResult game_result)
        {
            CheckmateCache.Clear();
            _chess_vm.ClickedOnCell -= ClickedOnCell;
            _chess_vm.ResultOfPawnChangeObtained -= ResultOfPawnChangeObtained;
            GameOver?.Invoke(this, game_result);
        }
        /// <summary>
        /// Обработчик события "Игра сохранена"
        /// </summary>
        /// <param name="sender">Источник события</param>
        private void VMGameSaved(object sender)
        {
            CheckmateCache.Clear();
            _chess_vm.ClickedOnCell -= ClickedOnCell;
            _chess_vm.ResultOfPawnChangeObtained -= ResultOfPawnChangeObtained;
            EventsDetached?.Invoke(this);
        }
        /// <summary>
        /// Обрабочтик события "Результат смены пешки получен"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="figure_position">Позиция фигуры</param>
        /// <param name="figure_color">Цвет фигуры</param>
        /// <param name="change_result">Результирующая фигура</param>
        private void ResultOfPawnChangeObtained(object sender, Position figure_position, FigureColor figure_color, ChangeResult change_result)
        {
            Figure figure = null;
            switch (change_result)
            {
                case ChangeResult.Queen: { figure = new Queen(figure_position, figure_color); break; }
                case ChangeResult.Bishop: { figure = new Bishop(figure_position, figure_color); break; }
                case ChangeResult.Knight: { figure = new Knight(figure_position, figure_color); break; }
                case ChangeResult.Rook: { figure = new Rook(figure_position, figure_color); break; }
            }
            figure.Moved += FigureMoved;
            figure.Attacked += FigureAttacked;
            GetCellInPosition(figure_position).Figure = figure;
            for (int i = 0; i < _figures.GetLength(0); i++)
            {
                for (int j = 0; j < _figures.GetLength(1); j++)
                {
                    if (_figures[i, j] != null && _figures[i, j].Position == figure_position) _figures[i, j] = figure;
                }
            }
            Checkmate();
            ReverseCurrentMoveColor();
        }
        /// <summary>
        /// Обработчик события "Клик по клетке доски"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="cell">Клетка доски</param>
        private void ClickedOnCell(object sender, Cell cell)
        {
            Cell clicked_cell = cell;
            Cell active_cell = _cells.Cast<Cell>().FirstOrDefault(c => c.Active);
            if (active_cell is null)
            {
                clicked_cell.Active = true;
                if (clicked_cell.Figure != null)
                {
                    if (clicked_cell.Figure.Color != CurrentMoveColor) return;
                    AddPossibleMoves(CheckmateCache.CachedMoves.Count == 0 ? 
                        GetFiltredPossiblePositions(clicked_cell.Figure) : 
                        CheckmateCache.CachedMoves.Find(p => p.figure == clicked_cell.Figure).possible_moves);
                }
            }
            else
            {
                if (clicked_cell == active_cell)
                {
                    clicked_cell.Active = false;
                    ResetPossibleMoves();
                }
                else
                {
                    active_cell.Active = false;
                    ResetPossibleMoves();

                    if (clicked_cell.Figure != null)
                    {
                        if (active_cell.Figure == null)
                        {
                            clicked_cell.Active = true;
                            if (clicked_cell.Figure.Color != CurrentMoveColor) return;
                            List<Position> possible_moves = CheckmateCache.CachedMoves.Count == 0 ? 
                                GetFiltredPossiblePositions(clicked_cell.Figure) : 
                                CheckmateCache.CachedMoves.Find(p => p.figure == clicked_cell.Figure).possible_moves;
                            AddPossibleMoves(possible_moves);
                        }
                        else
                        {
                            List<Position> possible_moves_active = CheckmateCache.CachedMoves.Count == 0 ? 
                                GetFiltredPossiblePositions(active_cell.Figure) :
                                CheckmateCache.CachedMoves.Find(p => p.figure == active_cell.Figure).possible_moves;
                            if (active_cell.Figure.Color != CurrentMoveColor) return;
                            if (possible_moves_active.Contains(clicked_cell.Position))
                            {
                                active_cell.Figure.AttackTo(clicked_cell.Position);
                            }
                            else
                            {
                                ResetPossibleMoves();
                                clicked_cell.Active = true;
                                if (clicked_cell.Figure.Color != CurrentMoveColor) return;
                                List<Position> possible_moves_clicked = CheckmateCache.CachedMoves.Count == 0 ?
                                GetFiltredPossiblePositions(clicked_cell.Figure) :
                                CheckmateCache.CachedMoves.Find(p => p.figure == clicked_cell.Figure).possible_moves;
                                AddPossibleMoves(possible_moves_clicked);
                            }
                        }
                    }
                    else
                    {
                        if (active_cell.Figure == null)
                        {
                            clicked_cell.Active = true;
                        }
                        else
                        {
                            List<Position> possible_moves = CheckmateCache.CachedMoves.Count == 0 ? 
                                GetFiltredPossiblePositions(active_cell.Figure) :
                                CheckmateCache.CachedMoves.Find(p => p.figure == active_cell.Figure).possible_moves;
                            if (!(possible_moves.Contains(clicked_cell.Position)))
                            {
                                clicked_cell.Active = true;
                            }
                            else
                            {
                                if (active_cell.Figure.Color != CurrentMoveColor) return;
                                if (active_cell.Figure is Pawn && (active_cell.Figure as Pawn).HasEnPassant)
                                {
                                    Pawn active_figure = (Pawn)active_cell.Figure;
                                    Position tmp_pos = active_figure.Color == FigureColor.White ?
                                        new Position(clicked_cell.Position.X - 1, clicked_cell.Position.Y) :
                                        new Position(clicked_cell.Position.X + 1, clicked_cell.Position.Y);

                                    if (tmp_pos != active_figure.Position)
                                    {
                                        active_figure.EnPassantAttack(tmp_pos);
                                    }
                                    else
                                    {
                                        active_cell.Figure.MoveTo(clicked_cell.Position, CountMoves);
                                    }
                                }
                                else if (active_cell.Figure is King && (active_cell.Figure as King).HasCastle)
                                {
                                    King active_figure = (King)active_cell.Figure;
                                    if (clicked_cell.Position.Y == 2)
                                    {
                                        active_figure.ToCastle(clicked_cell.Position, new Position(active_figure.Position.X, 0), new Position(active_figure.Position.X, 3));
                                    }
                                    else if (clicked_cell.Position.Y == 6)
                                    {
                                        active_figure.ToCastle(clicked_cell.Position, new Position(active_figure.Position.X, 7), new Position(active_figure.Position.X, 5));
                                    }
                                    else
                                    {
                                        active_cell.Figure.MoveTo(clicked_cell.Position, CountMoves);
                                    }
                                }
                                else
                                {
                                    active_cell.Figure.MoveTo(clicked_cell.Position, CountMoves);
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Обработчик события "Пешка дошла до границы доски"
        /// </summary>
        /// <param name="sender">Источник события</param>
        private void PawnChanged(object sender)
        {
            if (!(sender is Figure)) return;
            Figure figure = (Figure)sender;
            PawnChangedResult?.Invoke(this, figure.Position, figure.Color);
        }
        /// <summary>
        /// Обработчик события "Рокировка"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="king_position">Позиция короля</param>
        /// <param name="king_movable_position">Позиция на которую перемещается король</param>
        /// <param name="rook_position">Позиция ладьи</param>
        /// <param name="rook_movable_position">Позиция на которую перемещается ладья</param>
        private void KingToCastled(object sender, Position king_position, Position king_movable_position, Position rook_position, Position rook_movable_position)
        {
            if (!(sender is King)) return;
            King king = (King)sender;
            Rook rook = GetCellInPosition(rook_position).Figure as Rook;
            rook.Position = rook_movable_position;
            GetCellInPosition(king_position).Figure = null;
            GetCellInPosition(king_movable_position).Figure = king;
            GetCellInPosition(rook_position).Figure = null;
            GetCellInPosition(rook_movable_position).Figure = rook;
            king.HasCastle = false;
            ReverseCurrentMoveColor();
            Checkmate();
        }
        /// <summary>
        /// Обработчик события "Взятие на проходе"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="attacking_figure_position">Позиция атакующей фигуры</param>
        /// <param name="position_figure_begin_attacked">Позиция атакуемой фигуры</param>
        /// <param name="end_passant_position">Окончательная позиция атакующей фигуры</param>
        private void PawnEnPassanted(object sender, Position attacking_figure_position, Position position_figure_begin_attacked, Position end_passant_position)
        {
            if (!(sender is Pawn)) return;
            Pawn attacking_figure = (Pawn)sender;
            Figure attacked_figure = GetCellInPosition(position_figure_begin_attacked).Figure;
            GetCellInPosition(end_passant_position).Figure = attacking_figure;
            GetCellInPosition(attacking_figure_position).Figure = null;
            GetCellInPosition(position_figure_begin_attacked).Figure = null;
            for (int i = 0; i < _figures.GetLength(0); i++)
            {
                for (int j = 0; j < _figures.GetLength(1); j++)
                {
                    if (_figures[i, j] == attacked_figure) _figures[i, j] = null;
                }
            }
            attacking_figure.HasEnPassant = false;
            CountMoves++;
            ReverseCurrentMoveColor();
            Checkmate();
        }
        /// <summary>
        /// Обработчик события "Фигура атакует"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="attacked_from">Позиция атакующей фигуры</param>
        /// <param name="attacked_to">Позиция атаки</param>
        private void FigureAttacked(object sender, Position attacked_from, Position attacked_to)
        {
            if (!(sender is Figure)) return;
            Figure current_figure = (Figure)sender;
            Cell from = GetCellInPosition(attacked_from);
            Cell to = GetCellInPosition(attacked_to);
            for (int i = 0; i < _figures.GetLength(0); i++)
            {
                for (int j = 0; j < _figures.GetLength(1); j++)
                {
                    if (_figures[i,j] != null && _figures[i,j] != current_figure && _figures[i, j].Position == attacked_to) _figures[i, j] = null;
                }
            }
            from.Figure = null;
            to.Figure = current_figure;
            CountMoves++;
            if (current_figure.Position.X != (current_figure.Color == FigureColor.White ? 7 : 0) || !(current_figure is Pawn))
            {
                ReverseCurrentMoveColor();
                Checkmate();
            }
        }
        /// <summary>
        /// Обработчик события "Передвижение фигуры"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="moved_from">Позиция передвигающейся фигуры</param>
        /// <param name="moved_to">Позиция передвижения</param>
        private void FigureMoved(object sender, Position moved_from, Position moved_to)
        {
            if (!(sender is Figure)) return;
            Figure current_figure = (Figure)sender;
            Cell from = GetCellInPosition(moved_from);
            Cell to = GetCellInPosition(moved_to);
            from.Figure = null;
            to.Figure = current_figure;
            CountMoves++;
            if (current_figure.Position.X != (current_figure.Color == FigureColor.White ? 7 : 0) || !(current_figure is Pawn))
            {
                ReverseCurrentMoveColor();
                Checkmate();
            }
        }
        /// <summary>
        /// Кэширование возможных ходов с проверкой на мат
        /// </summary>
        private void Checkmate()
        {
            CheckmateCache.Clear();
            List<Cell> cells = _cells.Cast<Cell>().ToList();
            List<Figure> figures = _figures.Cast<Figure>().Where(e => e != null).ToList();
            ConcurrentBag<(Figure figure, List<Position> possible_moves)> possible_moves = new ConcurrentBag<(Figure figure, List<Position> possible_moves)>();
            Parallel.ForEach(figures, e => possible_moves.Add((e, GetFiltredPossiblePositions(e))));
            List<(Figure figure, List<Position> possible_moves)> white_possible_moves = possible_moves.Where(e => e.figure.Color == FigureColor.White).ToList();
            List<(Figure figure, List<Position> possible_moves)> black_possible_moves = possible_moves.Where(e => e.figure.Color == FigureColor.Black).ToList();
            if (CurrentMoveColor == FigureColor.White)
            {
                (Figure white_king, List<Position> white_king_possible_moves) = white_possible_moves.Find(e => e.figure is King);
                List<Position> white_king_possible_moves_copy = white_king_possible_moves.Select(e => e).ToList();
                ConcurrentBag<(Figure figure, List<Position> pseudo_possible_moves)> black_pseudo_possible_moves = new ConcurrentBag<(Figure figure, List<Position> pseudo_possible_moves)>();
                Parallel.ForEach(black_possible_moves.Select(e => e.figure), e => black_pseudo_possible_moves.Add((e, e.GetPossibleMoves())));
                white_king_possible_moves_copy.ForEach(e => 
                { 
                    if (black_possible_moves.Any(p => 
                    {
                        if (p.figure is Pawn)
                        {
                            return p.figure.Position.X - 1 == e.X && (p.figure.Position.Y + 1 == e.Y || p.figure.Position.Y - 1 == e.Y);
                        }
                        else return p.possible_moves.Contains(e);
                    })) white_king_possible_moves.Remove(e);
                    if(black_possible_moves.Any(p => p.figure.Position == e))
                    {
                        var pseudo_attacking_figures = black_pseudo_possible_moves.Where(ps_pos => ps_pos.pseudo_possible_moves.Contains(e));
                        if(pseudo_attacking_figures.Any(paf =>
                        {
                            if (paf.figure is Knight) return true;
                            var pos_between = GetPositionBetween(e, paf.figure.Position);
                            if (pos_between.Count == 0) return true;
                            return GetFiguresInPositions(pos_between).Count == 0;
                        })) white_king_possible_moves.Remove(e);
                    }
                });
                foreach (var (figure, pseudo_possible_moves) in black_pseudo_possible_moves)
                {
                    if(pseudo_possible_moves.Contains(white_king.Position) && !(figure is Knight))
                    {
                        List<Position> pos_between = GetPositionBetween(white_king.Position, figure.Position);
                        List<Figure> figures_between = GetFiguresInPositions(pos_between);
                        if (figures_between.Count == 1 && figures_between[0].Color == FigureColor.White)
                        {
                            var defensive_figure = white_possible_moves.Find(e => e.figure == figures_between[0]);
                            var defensive_figure_possible_moves = pseudo_possible_moves.Intersect(defensive_figure.possible_moves).Intersect(pos_between).ToList();
                            defensive_figure.possible_moves.Clear();
                            defensive_figure.possible_moves.AddRange(defensive_figure_possible_moves);
                        }
                    }
                }
                List<(Figure figure, List<Position> possible_moves)> checkmating_figures = black_possible_moves.FindAll(e => e.possible_moves.Contains(white_king.Position));
                if(checkmating_figures.Count > 0)
                {
                    if(checkmating_figures.Count >= 2)
                    {
                        white_possible_moves.ForEach(e =>
                        {
                            if (e.figure != white_king) e.possible_moves.Clear();
                        });
                    }
                    else
                    {
                        List<Position> pos_between = GetPositionBetween(white_king.Position, checkmating_figures[0].figure.Position);
                        var defensive_figures = white_possible_moves.FindAll(e => e.possible_moves.Any(p => pos_between.Contains(p)));
                        defensive_figures.ForEach(e => 
                        { 
                            var defenvise_pos = e.possible_moves.Find(p => pos_between.Contains(p));
                            e.possible_moves.Clear(); 
                            if(!(checkmating_figures[0].figure is Knight)) e.possible_moves.Add(defenvise_pos);
                        });
                        var attack_figures = white_possible_moves.FindAll(e => e.possible_moves.Contains(checkmating_figures[0].figure.Position));
                        attack_figures.ForEach(e => { var attack_pos = e.possible_moves.Find(p => p == checkmating_figures[0].figure.Position); e.possible_moves.Clear(); e.possible_moves.Add(attack_pos); });
                        white_possible_moves.ForEach(e =>
                        {
                            if (e.figure != white_king &&
                            !defensive_figures.Select(d => d.figure).Contains(e.figure) &&
                            !attack_figures.Select(a => a.figure).Contains(e.figure)) e.possible_moves.Clear();
                        });
                    }
                    if (white_possible_moves.TrueForAll(e => e.possible_moves.Count == 0))
                    {
                        CheckmateCache.Clear();
                        _chess_vm.ClickedOnCell -= ClickedOnCell;
                        _chess_vm.ResultOfPawnChangeObtained -= ResultOfPawnChangeObtained;
                        GameOver.Invoke(this, GameResult.BlackWin);
                        return;
                    }
                }
                else
                {
                    if (white_possible_moves.TrueForAll(e => e.possible_moves.Count == 0)) 
                    {
                        CheckmateCache.Clear();
                        _chess_vm.ClickedOnCell -= ClickedOnCell;
                        _chess_vm.ResultOfPawnChangeObtained -= ResultOfPawnChangeObtained;
                        GameOver.Invoke(this, GameResult.Draw);
                    }
                }
            }
            else
            {
                (Figure black_king, List<Position> black_king_possible_moves) = black_possible_moves.Find(e => e.figure is King);
                List<Position> black_king_possible_moves_copy = black_king_possible_moves.Select(e => e).ToList();
                ConcurrentBag<(Figure figure, List<Position> pseudo_possible_moves)> white_pseudo_possible_moves = new ConcurrentBag<(Figure figure, List<Position> pseudo_possible_moves)>();
                Parallel.ForEach(white_possible_moves.Select(e => e.figure), e => white_pseudo_possible_moves.Add((e, e.GetPossibleMoves())));
                black_king_possible_moves_copy.ForEach(e =>
                {
                    if (white_possible_moves.Any(p =>
                    {
                        if (p.figure is Pawn)
                        {
                            return p.figure.Position.X + 1 == e.X && (p.figure.Position.Y + 1 == e.Y || p.figure.Position.Y - 1 == e.Y);
                        }
                        else return p.possible_moves.Contains(e);
                    })) black_king_possible_moves.Remove(e);
                    if (white_possible_moves.Any(p => p.figure.Position == e))
                    {
                        var pseudo_attacking_figures = white_pseudo_possible_moves.Where(ps_pos => ps_pos.pseudo_possible_moves.Contains(e));
                        if (pseudo_attacking_figures.Any(paf =>
                        {
                            if (paf.figure is Knight) return true;
                            var pos_between = GetPositionBetween(e, paf.figure.Position);
                            if (pos_between.Count == 0) return true;
                            return GetFiguresInPositions(pos_between).Count == 0;
                        })) black_king_possible_moves.Remove(e);
                    }
                });
                foreach (var item in white_pseudo_possible_moves)
                {
                    if (item.pseudo_possible_moves.Contains(black_king.Position) && !(item.figure is Knight))
                    {
                        List<Position> pos_between = GetPositionBetween(black_king.Position, item.figure.Position);
                        List<Figure> figures_between = GetFiguresInPositions(pos_between);
                        if (figures_between.Count == 1 && figures_between[0].Color == FigureColor.Black) black_possible_moves.Find(e => e.figure == figures_between[0]).possible_moves.Clear();
                    }
                }
                List<(Figure figure, List<Position> possible_moves)> checkmating_figures = white_possible_moves.FindAll(e => e.possible_moves.Contains(black_king.Position));
                if (checkmating_figures.Count > 0)
                {
                    if (checkmating_figures.Count >= 2)
                    {
                        black_possible_moves.ForEach(e =>
                        {
                            if (e.figure != black_king) e.possible_moves.Clear();
                        });
                    }
                    else
                    {
                        List<Position> pos_between = GetPositionBetween(black_king.Position, checkmating_figures[0].figure.Position);
                        var defensive_figures = black_possible_moves.FindAll(e => e.possible_moves.Any(p => pos_between.Contains(p)));
                        defensive_figures.ForEach(e =>
                        {
                            var defenvise_pos = e.possible_moves.Find(p => pos_between.Contains(p));
                            e.possible_moves.Clear();
                            if (!(checkmating_figures[0].figure is Knight)) e.possible_moves.Add(defenvise_pos);
                        });
                        var attack_figures = black_possible_moves.FindAll(e => e.possible_moves.Contains(checkmating_figures[0].figure.Position));
                        attack_figures.ForEach(e => { var attack_pos = e.possible_moves.Find(p => p == checkmating_figures[0].figure.Position); e.possible_moves.Clear(); e.possible_moves.Add(attack_pos); });
                        black_possible_moves.ForEach(e =>
                        {
                            if (e.figure != black_king &&
                            !defensive_figures.Select(d => d.figure).Contains(e.figure) &&
                            !attack_figures.Select(a => a.figure).Contains(e.figure)) e.possible_moves.Clear();
                        });
                    }
                    if (black_possible_moves.TrueForAll(e => e.possible_moves.Count == 0))
                    {
                        CheckmateCache.Clear();
                        _chess_vm.ClickedOnCell -= ClickedOnCell;
                        _chess_vm.ResultOfPawnChangeObtained -= ResultOfPawnChangeObtained;
                        GameOver.Invoke(this, GameResult.WhiteWin);
                    }
                }
                else
                {
                    if (black_possible_moves.TrueForAll(e => e.possible_moves.Count == 0)) 
                    {
                        CheckmateCache.Clear();
                        _chess_vm.ClickedOnCell -= ClickedOnCell;
                        _chess_vm.ResultOfPawnChangeObtained -= ResultOfPawnChangeObtained;
                        GameOver.Invoke(this, GameResult.Draw);
                    }
                }
            }
            CheckmateCache.CachedMoves.AddRange(white_possible_moves);
            CheckmateCache.CachedMoves.AddRange(black_possible_moves);
        }
        /// <summary>
        /// Получить все фигуры в позициях
        /// </summary>
        /// <param name="positions">Список позиций</param>
        /// <returns>Список фигур находящихся на позиция</returns>
        private List<Figure> GetFiguresInPositions(List<Position> positions)
        {
            List<Figure> figures = _figures.Cast<Figure>().Where(e => e != null).ToList();
            return positions.Select(e => figures.Find(f => f.Position == e)).Where(e => e != null).ToList();
        }
        /// <summary>
        /// Получить все позиции между двумя позициями
        /// </summary>
        /// <param name="p1">Первая позиция</param>
        /// <param name="p2">Вторая позиция</param>
        /// <returns>Список позиций, находящихся между первой и второй позициями</returns>
        private List<Position> GetPositionBetween(Position p1, Position p2)
        {
            List<Position> result = new List<Position>();
            if (p1.X == p2.X && p1.Y == p2.Y) return result;
            if (p2.X > p1.X && p2.Y > p1.Y)
            {
                for (int x = p1.X + 1, y = p1.Y + 1; x < p2.X && y < p2.Y; x++, y++)
                {
                    result.Add(new Position(x, y));
                }
            }
            else if (p2.X > p1.X && p2.Y < p1.Y)
            {
                for (int x = p1.X + 1, y = p1.Y - 1; x < p2.X && y > p2.Y; x++, y--)
                {
                    result.Add(new Position(x, y));
                }
            }
            else if (p2.X < p1.X && p2.Y < p1.Y)
            {
                for (int x = p1.X - 1, y = p1.Y - 1; x > p2.X && y > p2.Y; x--, y--)
                {
                    result.Add(new Position(x, y));
                }
            }
            else if (p2.X < p1.X && p2.Y > p1.Y)
            {
                for (int x = p1.X - 1, y = p1.Y + 1; x > p2.X && y < p2.Y; x--, y++)
                {
                    result.Add(new Position(x, y));
                }
            }
            else if (p2.X > p1.X && p2.Y == p1.Y)
            {
                for (int x = p1.X + 1; x < p2.X; x++)
                {
                    result.Add(new Position(x, p1.Y));
                }
            }
            else if (p2.X < p1.X && p2.Y == p1.Y)
            {
                for (int x = p1.X - 1; x > p2.X; x--)
                {
                    result.Add(new Position(x, p1.Y));
                }
            }
            else if (p2.X == p1.X && p2.Y > p1.Y)
            {
                for (int y = p1.Y + 1; y < p2.Y; y++)
                {
                    result.Add(new Position(p1.X, y));
                }
            }
            else
            {
                for (int y = p1.Y - 1; y > p2.Y; y--)
                {
                    result.Add(new Position(p1.X, y));
                }
            }
            return result;
        }
        /// <summary>
        /// Начальная конфигурация доски
        /// </summary>
        private void BoardStartSetup()
        {
            CurrentMoveColor = FigureColor.White;
            for (int i = 0; i < _cells.GetLength(0); i++)
            {
                for (int j = 0; j < _cells.GetLength(1); j++)
                {
                    CellColors color = (i + j) % 2 == 0 ? CellColors.White : CellColors.Black;
                    _cells[i, j] = new Cell(color, false, new Position(7 - i, j));
                }
            }
            for (int i = 0; i < _figures.GetLength(1); i++)
            {
                Pawn black_pawn = new Pawn(new Position(6, i), FigureColor.Black);
                Pawn white_pawn = new Pawn(new Position(1, i), FigureColor.White);
                black_pawn.EnPassanted += PawnEnPassanted;
                white_pawn.EnPassanted += PawnEnPassanted;
                black_pawn.Changed += PawnChanged;
                white_pawn.Changed += PawnChanged;
                _figures[1, i] = black_pawn;
                _figures[2, i] = white_pawn;
                _cells[1, i].Figure = black_pawn;
                _cells[6, i].Figure = white_pawn;
            }
            King black_king = new King(new Position(7, 4), FigureColor.Black);
            King white_king = new King(new Position(0, 4), FigureColor.White);
            black_king.ToCastled += KingToCastled;
            white_king.ToCastled += KingToCastled;

            _figures[0, 0] = new Rook(new Position(7, 0), FigureColor.Black);
            _figures[0, 1] = new Knight(new Position(7, 1), FigureColor.Black);
            _figures[0, 2] = new Bishop(new Position(7, 2), FigureColor.Black);
            _figures[0, 3] = new Queen(new Position(7, 3), FigureColor.Black);
            _figures[0, 4] = black_king;
            _figures[0, 5] = new Bishop(new Position(7, 5), FigureColor.Black);
            _figures[0, 6] = new Knight(new Position(7, 6), FigureColor.Black);
            _figures[0, 7] = new Rook(new Position(7, 7), FigureColor.Black);

            _figures[3, 0] = new Rook(new Position(0, 0), FigureColor.White);
            _figures[3, 1] = new Knight(new Position(0, 1), FigureColor.White);
            _figures[3, 2] = new Bishop(new Position(0, 2), FigureColor.White);
            _figures[3, 3] = new Queen(new Position(0, 3), FigureColor.White);
            _figures[3, 4] = white_king;
            _figures[3, 5] = new Bishop(new Position(0, 5), FigureColor.White);
            _figures[3, 6] = new Knight(new Position(0, 6), FigureColor.White);
            _figures[3, 7] = new Rook(new Position(0, 7), FigureColor.White);

            for (int i = 0; i < _figures.GetLength(1); i++)
            {
                _cells[0, i].Figure = _figures[0, i];
                _cells[7, i].Figure = _figures[3, i];
            }
        }
        /// <summary>
        /// Восстановление состояния доски
        /// </summary>
        /// <param name="restore_state_info">Информация о состоянии доски в виде строки</param>
        private void RestoreState(string restore_state_info)
        {
            for (int i = 0; i < _cells.GetLength(0); i++)
            {
                for (int j = 0; j < _cells.GetLength(1); j++)
                {
                    CellColors color = (i + j) % 2 == 0 ? CellColors.White : CellColors.Black;
                    _cells[i, j] = new Cell(color, false, new Position(7 - i, j));
                }
            }
            string[] info = restore_state_info.Split('*');
            if (info.Length != 34) return;
            CurrentMoveColor = (FigureColor)Enum.Parse(typeof(FigureColor), info[0]);
            CountMoves = int.Parse(info[1]);
            for (int i = 2; i < info.Length; i++)
            {
                int figure_row_index = (i - 2) / 8;
                int figure_column_index = (i - 2) % 8;
                if (info[i] == "empty") _figures[figure_row_index, figure_column_index] = null;
                else
                {
                    string[] figure_info = info[i].Split(' ');
                    string figure_type = figure_info[0];
                    string[] figure_pos_info = figure_info[1].Split('|');
                    Position figure_pos = new Position(int.Parse(figure_pos_info[0]), int.Parse(figure_pos_info[1]));
                    FigureColor figure_color = (FigureColor)Enum.Parse(typeof(FigureColor), figure_info[2]);
                    switch (figure_type)
                    {
                        case "Rook":
                            {
                                Rook figure = new Rook(figure_pos, figure_color);
                                _figures[figure_row_index, figure_column_index] = figure;
                                Cell cell = GetCellInPosition(figure_pos);
                                cell.Figure = figure;
                                break;
                            }
                        case "Queen":
                            {
                                Queen figure = new Queen(figure_pos, figure_color);
                                _figures[figure_row_index, figure_column_index] = figure;
                                Cell cell = GetCellInPosition(figure_pos);
                                cell.Figure = figure;
                                break;
                            }
                        case "Bishop":
                            {
                                Bishop figure = new Bishop(figure_pos, figure_color);
                                _figures[figure_row_index, figure_column_index] = figure;
                                Cell cell = GetCellInPosition(figure_pos);
                                cell.Figure = figure;
                                break;
                            }
                        case "Knight":
                            {
                                Knight figure = new Knight(figure_pos, figure_color);
                                _figures[figure_row_index, figure_column_index] = figure;
                                Cell cell = GetCellInPosition(figure_pos);
                                cell.Figure = figure;
                                break;
                            }
                        case "Pawn":
                            {
                                MovementsState movement_state = (MovementsState)Enum.Parse(typeof(MovementsState), figure_info[3]);
                                bool has_en_passant = bool.Parse(figure_info[4]);
                                int en_passant_number_move = int.Parse(figure_info[5]);
                                Pawn figure = new Pawn(figure_pos, figure_color, movement_state, has_en_passant, en_passant_number_move);
                                figure.EnPassanted += PawnEnPassanted;
                                figure.Changed += PawnChanged;
                                _figures[figure_row_index, figure_column_index] = figure;
                                Cell cell = GetCellInPosition(figure_pos);
                                cell.Figure = figure;
                                break;
                            }
                        case "King":
                            {
                                MovementsState movement_state = (MovementsState)Enum.Parse(typeof(MovementsState), figure_info[3]);
                                bool has_castle = bool.Parse(figure_info[4]);
                                King figure = new King(figure_pos, figure_color, movement_state, has_castle);
                                figure.ToCastled += KingToCastled;
                                _figures[figure_row_index, figure_column_index] = figure;
                                Cell cell = GetCellInPosition(figure_pos);
                                cell.Figure = figure;
                                break;
                            }
                    }
                }
            }
            _figures.Cast<Figure>().ToList().ForEach(e => { if (e != null) { e.Moved += FigureMoved; e.Attacked += FigureAttacked; } });
        }
        /// <summary>
        /// Добавление возможных ходов выбранной фигуры на доску
        /// </summary>
        /// <param name="possible_positions">Список возможных ходов</param>
        private void AddPossibleMoves(List<Position> possible_positions)
        {
            foreach (Position position in possible_positions)
            {
                Cell cell = GetCellInPosition(position);
                if(cell != null)
                {
                    cell.IsPossible = true;
                }
            }
        }
        /// <summary>
        /// Сбрасывание возможных ходов выбранной фигуры
        /// </summary>
        private void ResetPossibleMoves()
        {
            for (int i = 0; i < _cells.GetLength(0); i++)
            {
                for (int j = 0; j < _cells.GetLength(1); j++)
                {
                    _cells[i, j].IsPossible = false;
                }
            }
        }
        /// <summary>
        /// Получить клетку доски в позиции
        /// </summary>
        /// <param name="position">Позиция</param>
        /// <returns>Клетка доски</returns>
        private Cell GetCellInPosition(Position position)
            => _cells.Cast<Cell>().First(e => e.Position == position);
        /// <summary>
        /// Получить отфильтрованные возможные ходы фигуры с учетом ситуации на доске
        /// </summary>
        /// <param name="figure">Фигура</param>
        /// <returns>Список возможных ходов</returns>
        private List<Position> GetFiltredPossiblePositions(Figure figure)
        {
            FigureColor figure_color = figure.Color;
            Position figure_position = figure.Position;
            List<Position> tmp_pos = figure.GetPossibleMoves();
            List<Position> tmp_pos_with_figures = tmp_pos.Where(e => GetCellInPosition(e).Figure != null).ToList();
            foreach (var pos in tmp_pos_with_figures)
            {
                Figure figure_in_pos = GetCellInPosition(pos).Figure;
                if (figure_in_pos.Color == figure_color) tmp_pos.Remove(pos);
                if (pos.X > figure_position.X)
                {
                    if(pos.Y > figure_position.Y)
                    {
                        tmp_pos.RemoveAll(e => (e.X >= pos.X && e.Y >= pos.Y) && e != pos);
                    }
                    else if (pos.Y < figure_position.Y)
                    {
                        tmp_pos.RemoveAll(e => (e.X >= pos.X && e.Y <= pos.Y) && e != pos);
                    }
                    else
                    {
                        tmp_pos.RemoveAll(e => (e.X >= pos.X && e.Y == pos.Y) && e != pos);
                    }
                }
                else if(pos.X < figure_position.X)
                {
                    if (pos.Y > figure_position.Y)
                    {
                        tmp_pos.RemoveAll(e => (e.X <= pos.X && e.Y >= pos.Y) && e != pos);
                    }
                    else if (pos.Y < figure_position.Y)
                    {
                        tmp_pos.RemoveAll(e => (e.X <= pos.X && e.Y <= pos.Y) && e != pos);
                    }
                    else
                    {
                        tmp_pos.RemoveAll(e => (e.X <= pos.X && e.Y == pos.Y) && e != pos);
                    }
                }
                else
                {
                    if (pos.Y > figure_position.Y)
                    {
                        tmp_pos.RemoveAll(e => (e.X == pos.X && e.Y >= pos.Y) && e != pos);
                    }
                    else
                    {
                        tmp_pos.RemoveAll(e => (e.X == pos.X && e.Y <= pos.Y) && e != pos);
                    }
                }
            }

            if(figure is Pawn)
            {
                Pawn current_pawn_figure = (Pawn)figure;
                if (tmp_pos.Count != 0 && GetCellInPosition(tmp_pos[0]).Figure != null) tmp_pos.Clear();
                if (figure_color == FigureColor.White)
                {
                    #region En Passant (Взятие на проходе)
                    if (figure_position.X == 4)
                    {
                        Pawn en_passant_pawn_left = null;
                        try
                        {
                            en_passant_pawn_left = GetCellInPosition(new Position(figure_position.X, figure_position.Y - 1)).Figure as Pawn;
                        } catch { }
                        Pawn en_passant_pawn_right = null;
                        try
                        {
                            en_passant_pawn_right = GetCellInPosition(new Position(figure_position.X, figure_position.Y + 1)).Figure as Pawn;
                        } catch { }
                        if (en_passant_pawn_left != null && en_passant_pawn_left.MovementsState == MovementsState.One && en_passant_pawn_left.EnPassantNumberMove == CountMoves - 1)
                        {
                            tmp_pos.Add(new Position(en_passant_pawn_left.Position.X + 1, en_passant_pawn_left.Position.Y));
                            current_pawn_figure.HasEnPassant = true;
                        }
                        if(en_passant_pawn_right != null && en_passant_pawn_right.MovementsState == MovementsState.One && en_passant_pawn_right.EnPassantNumberMove == CountMoves - 1)
                        {
                            tmp_pos.Add(new Position(en_passant_pawn_right.Position.X + 1, en_passant_pawn_right.Position.Y));
                            current_pawn_figure.HasEnPassant = true;
                        }
                    }
                    #endregion

                    try
                    {
                        Cell tmp_cell = GetCellInPosition(new Position(figure_position.X + 1, figure_position.Y - 1));
                        if (tmp_cell.Figure != null && tmp_cell.Figure.Color != figure_color) tmp_pos.Add(tmp_cell.Position);
                    }
                    catch { }
                    try
                    {
                        Cell tmp_cell = GetCellInPosition(new Position(figure_position.X + 1, figure_position.Y + 1));
                        if (tmp_cell.Figure != null && tmp_cell.Figure.Color != figure_color) tmp_pos.Add(tmp_cell.Position);
                    }
                    catch { }
                    if(current_pawn_figure.MovementsState == MovementsState.Zero 
                        && GetCellInPosition(new Position(figure_position.X + 1, figure_position.Y)).Figure == null
                        && GetCellInPosition(new Position(figure_position.X + 2, figure_position.Y)).Figure == null)
                    {
                        tmp_pos.Add(new Position(figure_position.X + 2, figure_position.Y));
                    }
                }
                else
                {
                    #region En Passant (Взятие на проходе)
                    if (figure_position.X == 3)
                    {
                        Pawn en_passant_pawn_left = null;
                        try
                        {
                            en_passant_pawn_left = GetCellInPosition(new Position(figure_position.X, figure_position.Y - 1)).Figure as Pawn;
                        } catch { }
                        Pawn en_passant_pawn_right = null;
                        try
                        {
                            en_passant_pawn_right = GetCellInPosition(new Position(figure_position.X, figure_position.Y + 1)).Figure as Pawn;
                        } catch { }
                        if (en_passant_pawn_left != null && en_passant_pawn_left.MovementsState == MovementsState.One && en_passant_pawn_left.EnPassantNumberMove == CountMoves - 1)
                        {
                            tmp_pos.Add(new Position(en_passant_pawn_left.Position.X - 1, en_passant_pawn_left.Position.Y));
                            current_pawn_figure.HasEnPassant = true;
                        }
                        else if (en_passant_pawn_right != null && en_passant_pawn_right.MovementsState == MovementsState.One && en_passant_pawn_right.EnPassantNumberMove == CountMoves - 1)
                        {
                            tmp_pos.Add(new Position(en_passant_pawn_right.Position.X - 1, en_passant_pawn_right.Position.Y));
                            current_pawn_figure.HasEnPassant = true;
                        }
                    }
                    #endregion

                    try
                    {
                        Cell tmp_cell = GetCellInPosition(new Position(figure_position.X - 1, figure_position.Y - 1));
                        if (tmp_cell.Figure != null && tmp_cell.Figure.Color != figure_color) tmp_pos.Add(tmp_cell.Position);
                    }
                    catch { }
                    try
                    {
                        Cell tmp_cell = GetCellInPosition(new Position(figure_position.X - 1, figure_position.Y + 1));
                        if (tmp_cell.Figure != null && tmp_cell.Figure.Color != figure_color) tmp_pos.Add(tmp_cell.Position);
                    }
                    catch { }
                    if (current_pawn_figure.MovementsState == MovementsState.Zero
                        && GetCellInPosition(new Position(figure_position.X - 1, figure_position.Y)).Figure == null
                        && GetCellInPosition(new Position(figure_position.X - 2, figure_position.Y)).Figure == null)
                    {
                        tmp_pos.Add(new Position(figure_position.X - 2, figure_position.Y));
                    }
                }
            }

            #region Рокировка
            else if (figure is King)
            {
                King king = (King)figure;
                if(king.MovementsState == MovementsState.Zero && figure.Position.Y == 4)
                {
                    Rook left_rook = GetCellInPosition(new Position(figure_position.X, 0)).Figure as Rook;
                    Rook right_rook = GetCellInPosition(new Position(figure_position.X, 7)).Figure as Rook;
                    if (left_rook != null && left_rook.MovementsState == MovementsState.Zero)
                    {
                        if (GetCellInPosition(new Position(figure_position.X, 1)).Figure == null &&
                            GetCellInPosition(new Position(figure_position.X, 2)).Figure == null &&
                            GetCellInPosition(new Position(figure_position.X, 3)).Figure == null)
                        {
                            tmp_pos.Add(new Position(figure_position.X, 2));
                            (figure as King).HasCastle = true;
                        }
                    }
                    if (right_rook != null && right_rook.MovementsState == MovementsState.Zero)
                    {
                        if (GetCellInPosition(new Position(figure_position.X, 5)).Figure == null &&
                            GetCellInPosition(new Position(figure_position.X, 6)).Figure == null)
                        {
                            tmp_pos.Add(new Position(figure_position.X, 6));
                            (figure as King).HasCastle = true;
                        }
                    }
                }
            }
            #endregion

            return tmp_pos;
        }
        /// <summary>
        /// Изменить цвет текущего хода на противоположный.
        /// </summary>
        private void ReverseCurrentMoveColor()
        {
            switch (CurrentMoveColor)
            {
                case FigureColor.White: { CurrentMoveColor = FigureColor.Black; break; }
                case FigureColor.Black: { CurrentMoveColor = FigureColor.White; break; }
            }
        }
        /// <summary>
        /// Получить итератор, параметризированный классом Cell
        /// </summary>
        /// <returns>Итератор</returns>
        public IEnumerator<Cell> GetEnumerator()
        {
            return _cells.Cast<Cell>().GetEnumerator();
        }
        /// <summary>
        /// Получить итератор
        /// </summary>
        /// <returns>Итератор</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _cells.GetEnumerator();
        }
        /// <summary>
        /// Информация о доске в виде строки
        /// </summary>
        /// <returns>Строковое представление доски</returns>
        public override string ToString()
        {
            var figures = _figures.Cast<Figure>().ToList();
            string figure_info = string.Concat(figures.Select((e, i) => 
            {
                if(i == figures.Count - 1)
                {
                    return e is null ? "empty" : $"{e}";
                }
                else
                {
                    return e is null ? "empty*" : $"{e}*";
                }
            }));
            return $"{CurrentMoveColor}*{CountMoves}*{figure_info}";
        }
        #endregion
    }
}
