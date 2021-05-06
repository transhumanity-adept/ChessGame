namespace ChessGame.Model.Figures.Helpers
{
    class EnPassantedEventArgs
    {
        public Position AttackingFigurePosition { get; private set; }
        public Position PositionFigureBeginAttacked { get; private set; }
        public Position EndPassantPosition { get; private set; }

        public EnPassantedEventArgs(Position attacking_figure_position, Position position_figure_begin_attacked, Position end_passant_position)
            => (AttackingFigurePosition, PositionFigureBeginAttacked, EndPassantPosition)
            = (attacking_figure_position, position_figure_begin_attacked, end_passant_position);
    }
}
