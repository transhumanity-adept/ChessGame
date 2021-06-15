using ChessGame.Model.Figures.Helpers;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ChessGame.ViewModel.Converters
{
    class IsWhiteMoveConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((FigureColor)value) == FigureColor.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
