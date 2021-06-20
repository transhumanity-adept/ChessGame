using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using ChessGame.Model.Helpers;

namespace ChessGame.ViewModel.Converters
{
    /// <summary>
    /// Конвертер цвета клетки
    /// </summary>
    public class CellColorConverter : IValueConverter
    {
        #region Методы
        /// <summary>
        /// Конвертирование значения
        /// </summary>
        /// <returns>Сконвертированное значение</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((CellColors)value)
            {
                case CellColors.White: { return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EEEED2")); }
                case CellColors.Black: { return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#769656")); }
            }
            return null;
        }
        /// <summary>
        /// Обратное конвертирование
        /// </summary>
        /// <returns>Сконвертированное значение</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
        #endregion
    }
}
