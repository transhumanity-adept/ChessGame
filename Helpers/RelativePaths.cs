using System;
using System.IO;

namespace ChessGame.Helpers
{
    static class RelativePaths
    {
        public static string WhiteQueen { get; } = @"pack://application:,,,/ChessGame;component/Images/wqueen.png";
        public static string BlackQueen { get; } = @"pack://application:,,,/ChessGame;component/Images/bqueen.png";
        public static string WhiteKing { get; } = @"pack://application:,,,/ChessGame;component/Images/wking.png";
        public static string BlackKing { get; } = @"pack://application:,,,/ChessGame;component/Images/bking.png";
        public static string WhiteKnight { get; } = @"pack://application:,,,/ChessGame;component/Images/wknight.png";
        public static string BlackKnight { get; } = @"pack://application:,,,/ChessGame;component/Images/bknight.png";
        public static string WhiteRook { get; } = @"pack://application:,,,/ChessGame;component/Images/wrook.png";
        public static string BlackRook { get; } = @"pack://application:,,,/ChessGame;component/Images/brook.png";
        public static string WhiteBishop { get; } = @"pack://application:,,,/ChessGame;component/Images/wbishop.png";
        public static string BlackBishop { get; } = @"pack://application:,,,/ChessGame;component/Images/bbishop.png";
        public static string WhitePawn { get; } = @"pack://application:,,,/ChessGame;component/Images/wpawn.png";
        public static string BlackPawn { get; } = @"pack://application:,,,/ChessGame;component/Images/bpawn.png";
        public static string DataBaseConnection { get; } = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName}\Model\Data\ChessGameData.mdf;Integrated Security=True";
    }
}
