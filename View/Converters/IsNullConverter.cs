using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ChessGame.ViewModel.Converters
{
    /// <summary>
    /// Конвертер проверки на Null
    /// </summary>
    public class IsNullConverter : IValueConverter
    {
        #region Методы
        /// <summary>
        /// Конвертирование значения
        /// </summary>
        /// <returns>Сконвертированное значение</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is null;
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
