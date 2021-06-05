﻿using ChessGame.View;
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
            //Window window = new LoginRegistrationWindow();
            //window.ShowDialog();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(this.WindowState == WindowState.Maximized)
            {
                Left = 0;
                Top = 0;
            }
        }
    }
}
