using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ChessGame.Model;
using ChessGame.Model.Figures;
using ChessGame.Helpers;
using ChessGame.Model.Figures.Helpers;

namespace ChessGame.ViewModel
{
    class ChessViewModel : NotifyPropertyChanged
    {
        private Board _board;
        private ICommand _cell_click_command;
        public Board Board
        {
            get => _board;
            set
            {
                _board = value;
                OnPropertyChanged();
            }
        }

        public ICommand CellClickCommand => _cell_click_command ?? (_cell_click_command = new RelayCommand(obj =>
        {
            Cell clicked_cell = (Cell)obj;
            if (_board.FirstOrDefault(c => c.Active) is null)
            {
                clicked_cell.Active = true;
                if (clicked_cell.Figure != null)
                {
                    Figure figure = clicked_cell.Figure;
                    _board.AddPossibleMoves(_board.GetFiltredPossiblePositions(figure));
                }
            }
            else
            {
                // Выбранная ячейка является активной
                if (clicked_cell == _board.FirstOrDefault(c => c.Active))
                {
                    clicked_cell.Active = false;
                    _board.ResetPossibleMoves();
                }
                else
                {
                    Cell active_cell = _board.FirstOrDefault(c => c.Active);
                    active_cell.Active = false;
                    _board.ResetPossibleMoves();

                    if (clicked_cell.Figure != null)
                    {
                        if (active_cell.Figure == null)
                        {
                            clicked_cell.Active = true;
                            List<Position> possible_moves = _board.GetFiltredPossiblePositions(clicked_cell.Figure);
                            _board.AddPossibleMoves(possible_moves);
                        }
                        else
                        {
                            List<Position> possible_moves_active = _board.GetFiltredPossiblePositions(active_cell.Figure);
                            if (possible_moves_active.Contains(clicked_cell.Position))
                            {
                                active_cell.Figure.AttackTo(clicked_cell.Position);
                            }
                            else
                            {
                                _board.ResetPossibleMoves();
                                clicked_cell.Active = true;
                                List<Position> possible_moves_clicked = _board.GetFiltredPossiblePositions(clicked_cell.Figure);
                                _board.AddPossibleMoves(possible_moves_clicked);
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
                            List<Position> possible_moves = _board.GetFiltredPossiblePositions(active_cell.Figure);
                            if (!(possible_moves.Contains(clicked_cell.Position)))
                            {
                                clicked_cell.Active = true;
                            }
                            else
                            {
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
                                        active_cell.Figure.MoveTo(clicked_cell.Position, _board.CountMoves);
                                    }
                                } else if (active_cell.Figure is King && (active_cell.Figure as King).HasCastle)
                                {
                                    King active_figure = (King)active_cell.Figure;
                                    if(clicked_cell.Position.Y == 2)
                                    {
                                        active_figure.ToCastle(clicked_cell.Position, new Position(active_figure.Position.X, 0), new Position(active_figure.Position.X, 3));
                                    }
                                    else if(clicked_cell.Position.Y == 6)
                                    {
                                        active_figure.ToCastle(clicked_cell.Position, new Position(active_figure.Position.X, 7), new Position(active_figure.Position.X, 5));
                                    }
                                    else
                                    {
                                        active_cell.Figure.MoveTo(clicked_cell.Position, _board.CountMoves);
                                    }
                                }
                                else
                                {
                                    active_cell.Figure.MoveTo(clicked_cell.Position, _board.CountMoves);
                                }
                            }
                        }
                    }
                }
            }
        }, obj => obj is Cell));

        public ChessViewModel()
        {
            _board = new Board();
        }
    }
}
