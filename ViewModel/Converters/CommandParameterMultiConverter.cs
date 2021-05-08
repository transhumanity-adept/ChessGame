using ChessGame.Model;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ChessGame.ViewModel.Converters
{
    public class CommandParameterMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return new Tuple<Button, Cell>(values[0] as Button,values[1] as Cell);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return (object[])DependencyProperty.UnsetValue;
        }


    }
}
