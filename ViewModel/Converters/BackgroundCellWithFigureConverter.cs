using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using ChessGame.Model.Helpers;

namespace ChessGame.ViewModel.Converters
{
    /// <summary>
    /// Конвертер фона ячейки с фигурой
    /// </summary>
    public class BackgroundCellWithFigureConverter : IValueConverter
    {
        #region Методы
        /// <summary>
        /// Конвертирование значения
        /// </summary>
        /// <returns>Сконвертированное значение</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            RectangleGeometry radiuse_rect = new RectangleGeometry();
            RectangleGeometry background_rect = new RectangleGeometry();
            radiuse_rect.RadiusX = 200;
            radiuse_rect.RadiusY = 200;
            radiuse_rect.Rect = new Rect(25, 25, 450, 450);
            background_rect.Rect = new Rect(0, 0, 500, 500);
            Path radiuse_rect_path = new Path();
            Path background_rect_path = new Path();
            radiuse_rect_path.Data = radiuse_rect;
            background_rect_path.Data = background_rect;

            if ((CellColors)value == CellColors.White)
            {
                SolidColorBrush fill_color_brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b5b59f"));
                SolidColorBrush white_figure_color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EEEED2"));
                radiuse_rect_path.Fill = white_figure_color;
                background_rect_path.Fill = white_figure_color;
                DrawingGroup drawingGroup = new DrawingGroup();
                drawingGroup.Children.Add(new GeometryDrawing(white_figure_color, new Pen(Brushes.White, 0), background_rect));
                drawingGroup.Children.Add(new GeometryDrawing(white_figure_color, new Pen(fill_color_brush, 50), radiuse_rect));
                return new DrawingBrush(drawingGroup);
            }
            else
            {
                SolidColorBrush fill_color_brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#62754f"));
                SolidColorBrush black_figure_color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#769656"));
                radiuse_rect_path.Fill = black_figure_color;
                background_rect_path.Fill = black_figure_color;
                DrawingGroup drawingGroup = new DrawingGroup();
                drawingGroup.Children.Add(new GeometryDrawing(black_figure_color, new Pen(Brushes.White, 0), background_rect));
                drawingGroup.Children.Add(new GeometryDrawing(black_figure_color, new Pen(fill_color_brush, 50), radiuse_rect));
                return new DrawingBrush(drawingGroup);
            }
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
