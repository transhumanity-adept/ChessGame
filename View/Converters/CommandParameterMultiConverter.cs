using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ChessGame.ViewModel.Converters
{
    /// <summary>
    /// Конвертер параметром команды
    /// </summary>
    public class CommandParameterMultiConverter : IMultiValueConverter
    {
        #region Методы
        /// <summary>
        /// Конвертирование значения
        /// </summary>
        /// <returns>Сконвертированное значение</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return new Tuple<object, object>(values[0],values[1]);
        }
        /// <summary>
        /// Обратное конвертирование
        /// </summary>
        /// <returns>Сконвертированное значение</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return (object[])DependencyProperty.UnsetValue;
        }
        #endregion
    }
}
