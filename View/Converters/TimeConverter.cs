using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ChessGame.ViewModel.Converters
{
    /// <summary>
    /// Конвертер времени
    /// </summary>
    class TimeConverter : IValueConverter
    {
        #region Методы
        /// <summary>
        /// Конвертирование значения
        /// </summary>
        /// <returns>Сконвертированное значение</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TimeSpan.FromSeconds((int)double.Parse(value.ToString())).ToString();
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
