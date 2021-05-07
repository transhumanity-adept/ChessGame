using System.Windows;
using System.Windows.Media;

namespace ChessGame
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#312E2B"));
        }
    }
}
