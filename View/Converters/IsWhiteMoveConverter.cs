using ChessGame.Model.Figures.Helpers;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ChessGame.ViewModel.Converters
{
    /// <summary>
    /// Конвертер проверки текущего цвета
    /// </summary>
    class IsWhiteMoveConverter : IValueConverter
    {
        #region Методы
        /// <summary>
        /// Конвертирование значения
        /// </summary>
        /// <returns>Сконвертированное значение</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((FigureColor)value) == FigureColor.White;
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
