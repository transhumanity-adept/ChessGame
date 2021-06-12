using System.Collections.Generic;
using ChessGame.Model.Figures.Helpers;

namespace ChessGame.Model
{
    public abstract class Figure
    {
        protected Position _position;
        protected string _image_path;
        protected FigureColor _color;

        public delegate void FigureHandler(object sender, FigureEventArgs e);
        public event FigureHandler Moved;
        public event FigureHandler Attacked;
        public Position Position { get => _position; set => _position = value; }

        public virtual void MoveTo(Position new_position, int count_moves)
        {
            Position last_pos = Position;
            Position = new_position;
            Moved?.Invoke(this, new FigureEventArgs(last_pos, new_position));
        }

        public virtual void AttackTo(Position attack_position)
        {
            Position last_pos = Position;
            Position = attack_position;
            Attacked?.Invoke(this, new FigureEventArgs(last_pos, attack_position));
        }
        
        public string ImagePath
        {
            get => _image_path;
            private set => _image_path = value;
        }

        public FigureColor Color
        {
            get => _color;
            private set => _color = value;
        }

        public Figure(Position position, string image, FigureColor color)
            => (Position, ImagePath, Color) = (position, image, color);

        public abstract List<Position> GetPossibleMoves();
    }
}
