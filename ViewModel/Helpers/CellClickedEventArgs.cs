using ChessGame.Model;
namespace ChessGame.ViewModel.Helpers
{
    public class CellClickedEventArgs
    {
        public Cell ClickedCell { get; private set; }
        public CellClickedEventArgs(Cell clicked_cell) => ClickedCell = clicked_cell;
    }
}
