using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChessGame.Helpers;
using ChessGame.Model.Figures;
using ChessGame.Model.Figures.Helpers;

namespace ChessGame.Model
{
    public class PawnChangedEventArgs
    {
        public Position Position { get; private set; }
        public FigureColor Color { get; private set; }
        public PawnChangedEventArgs(Position pawn_pos, FigureColor color) => (Position, Color) = (pawn_pos, color);
    }
    class Board : IEnumerable<Cell>
    {
        private readonly Cell[,] _cells = new Cell[8, 8];
        private readonly Figure[,] _figures = new Figure[4, 8];
        private int _count_moves = 0;
        public Cell this[int row, int column]
        {
            get => _cells[row, column];
            set => _cells[row, column] = value;
        }
        public Board()
        {
            BoardStartSetup();
            _figures.Cast<Figure>().ToList().ForEach(e => { if (e != null) { e.Moved += Figure_Moved; e.Attacked += Figure_Attacked; } });
        }
        public delegate void PawnChangedHandler(object sender, PawnChangedEventArgs e);
        public event PawnChangedHandler PawnChanged;

        public int CountMoves
        {
            get => _count_moves;
            private set => _count_moves = value;
        }

        private void BoardStartSetup()
        {
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
                black_pawn.EnPassanted += Pawn_EnPassanted;
                white_pawn.EnPassanted += Pawn_EnPassanted;
                black_pawn.Changed += Pawn_Changed;
                white_pawn.Changed += Pawn_Changed;
                _figures[1, i] = black_pawn;
                _figures[2, i] = white_pawn;
                _cells[1, i].Figure = black_pawn;
                _cells[6, i].Figure = white_pawn;
            }
            King black_king = new King(new Position(7, 4), FigureColor.Black);
            King white_king = new King(new Position(0, 4), FigureColor.White);
            black_king.ToCastled += King_ToCastled;
            white_king.ToCastled += King_ToCastled;

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

        private void Pawn_Changed(object sender, EventArgs e)
        {
            if (!(sender is Figure)) return;
            Figure figure = (Figure)sender;
            PawnChanged?.Invoke(this, new PawnChangedEventArgs(figure.Position, figure.Color));
        }

        private void King_ToCastled(object sender, CastlingEventArgs e)
        {
            if (!(sender is King)) return;
            King king = (King)sender;
            Rook rook = GetCellInPosition(e.RookPosition).Figure as Rook;
            rook.Position = e.RookMovablePosition;
            GetCellInPosition(e.KingPosition).Figure = null;
            GetCellInPosition(e.KingMovablePosition).Figure = king;
            GetCellInPosition(e.RookPosition).Figure = null;
            GetCellInPosition(e.RookMovablePosition).Figure = rook;
            king.HasCastle = false;
        }

        private void Pawn_EnPassanted(object sender, EnPassantedEventArgs e)
        {
            if (!(sender is Pawn)) return;
            Pawn attacking_figure = (Pawn)sender;
            Figure attacked_figure = GetCellInPosition(e.PositionFigureBeginAttacked).Figure;
            (GetCellInPosition(e.EndPassantPosition)).Figure = attacking_figure;
            (GetCellInPosition(e.AttackingFigurePosition)).Figure = null;
            (GetCellInPosition(e.PositionFigureBeginAttacked)).Figure = null;
            for (int i = 0; i < _figures.GetLength(0); i++)
            {
                for (int j = 0; j < _figures.GetLength(1); j++)
                {
                    if (_figures[i, j] == attacking_figure) _figures[i, j] = null;
                    if (_figures[i, j] == attacked_figure) _figures[i, j] = null;
                }
            }
            attacking_figure.HasEnPassant = false;
            _count_moves++;
        }

        private void Figure_Attacked(object sender, FigureEventArgs e)
        {
            if (!(sender is Figure)) return;
            Cell to = GetCellInPosition(e.MovedTo);
            Figure attack_figure = to.Figure;
            for (int i = 0; i < _figures.GetLength(0); i++)
            {
                for (int j = 0; j < _figures.GetLength(1); j++)
                {
                    if (_figures[i, j] == attack_figure) _figures[i, j] = null;
                }
            }
        }

        private void Figure_Moved(object sender, FigureEventArgs e)
        {
            if (!(sender is Figure)) return;
            Figure current_figure = (Figure)sender;
            var cells = _cells.Cast<Cell>();
            Cell from = GetCellInPosition(e.MovedFrom);
            Cell to = GetCellInPosition(e.MovedTo);
            from.Figure = null;
            to.Figure = current_figure;
            _count_moves++;
        }

        public void PawnChange(Position position, FigureColor color, ChangeResult result)
        {
            Figure figure = null;
            switch (result)
            {
                case ChangeResult.Queen: { figure = new Queen(position, color); break; }
                case ChangeResult.Bishop: { figure = new Bishop(position, color); break; }
                case ChangeResult.Knight: { figure = new Knight(position, color); break; }
                case ChangeResult.Rook: { figure = new Rook(position, color); break; }
            }
            figure.Moved += Figure_Moved;
            figure.Attacked += Figure_Attacked;
            GetCellInPosition(position).Figure = figure;
        }
        public void AddPossibleMoves(List<Position> possible_positions)
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

        public void ResetPossibleMoves()
        {
            for (int i = 0; i < _cells.GetLength(0); i++)
            {
                for (int j = 0; j < _cells.GetLength(1); j++)
                {
                    _cells[i, j].IsPossible = false;
                }
            }
        }

        private Cell GetCellInPosition(Position position)
            => _cells.Cast<Cell>().First(e => e.Position == position);

        public List<Position> GetFiltredPossiblePositions(Figure figure)
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
                if (tmp_pos.Count == 0) return tmp_pos;
                if (GetCellInPosition(tmp_pos[0]).Figure != null) tmp_pos.Clear();
                if(figure_color == FigureColor.White)
                {
                    #region En Passant (Взятие на проходе)
                    if (figure_position.X == 4)
                    {
                        Pawn en_passant_pawn_left = GetCellInPosition(new Position(figure_position.X, figure_position.Y - 1)).Figure as Pawn;
                        Pawn en_passant_pawn_right = GetCellInPosition(new Position(figure_position.X, figure_position.Y + 1)).Figure as Pawn;
                        if (en_passant_pawn_left != null && en_passant_pawn_left.MovementsState == MovementsState.One && en_passant_pawn_left.EnPassantNumberMove == _count_moves - 1)
                        {
                            tmp_pos.Add(new Position(en_passant_pawn_left.Position.X + 1, en_passant_pawn_left.Position.Y));
                            current_pawn_figure.HasEnPassant = true;
                        }
                        if(en_passant_pawn_right != null && en_passant_pawn_right.MovementsState == MovementsState.One && en_passant_pawn_right.EnPassantNumberMove == _count_moves - 1)
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
                        Pawn en_passant_pawn_left = GetCellInPosition(new Position(figure_position.X, figure_position.Y - 1)).Figure as Pawn;
                        Pawn en_passant_pawn_right = GetCellInPosition(new Position(figure_position.X, figure_position.Y + 1)).Figure as Pawn;
                        if (en_passant_pawn_left != null && en_passant_pawn_left.MovementsState == MovementsState.One && en_passant_pawn_left.EnPassantNumberMove == _count_moves - 1)
                        {
                            tmp_pos.Add(new Position(en_passant_pawn_left.Position.X - 1, en_passant_pawn_left.Position.Y));
                            current_pawn_figure.HasEnPassant = true;
                        }
                        else if (en_passant_pawn_right != null && en_passant_pawn_right.MovementsState == MovementsState.One && en_passant_pawn_right.EnPassantNumberMove == _count_moves - 1)
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
            else if(figure is King && (figure as King).MovementsState == MovementsState.Zero && figure.Position.Y == 4)
            {
                Rook left_rook = GetCellInPosition(new Position(figure_position.X, 0)).Figure as Rook;
                Rook right_rook = GetCellInPosition(new Position(figure_position.X, 7)).Figure as Rook;
                if (left_rook != null && left_rook.MovementsState == MovementsState.Zero)
                {
                    if(GetCellInPosition(new Position(figure_position.X, 1)).Figure == null &&
                        GetCellInPosition(new Position(figure_position.X, 2)).Figure == null &&
                        GetCellInPosition(new Position(figure_position.X, 3)).Figure == null)
                    {
                        tmp_pos.Add(new Position(figure_position.X, 2));
                        (figure as King).HasCastle = true;
                    }
                }
                if(right_rook != null && right_rook.MovementsState == MovementsState.Zero)
                {
                    if (GetCellInPosition(new Position(figure_position.X, 5)).Figure == null &&
                        GetCellInPosition(new Position(figure_position.X, 6)).Figure == null)
                    {
                        tmp_pos.Add(new Position(figure_position.X, 6));
                        (figure as King).HasCastle = true;
                    }
                }
            }
            return tmp_pos;
        }

        public IEnumerator<Cell> GetEnumerator()
        {
            return _cells.Cast<Cell>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _cells.GetEnumerator();
        }
    }
}
