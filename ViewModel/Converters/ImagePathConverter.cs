using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;


namespace ChessGame.ViewModel.Converters
{
    /// <summary>
    /// Конвертер относительных путей изображений
    /// </summary>
    public class ImagePathConverter : IValueConverter
    {
        #region Методы
        /// <summary>
        /// Конвертирование значения
        /// </summary>
        /// <returns>Сконвертированное значение</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new ImageSourceConverter().ConvertFromString((string)value);
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
