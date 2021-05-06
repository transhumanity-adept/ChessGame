using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using ChessGame.Model;

namespace ChessGame.ViewModel.Converters
{
    public class CellColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((CellColors)value)
            {
                case CellColors.White: { return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EEEED2")); }
                case CellColors.Black: { return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#769656")); }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
