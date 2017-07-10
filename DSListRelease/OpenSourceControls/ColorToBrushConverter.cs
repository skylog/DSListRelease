using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DSList.OpenSourceControls
{
    //[ValueConversion(typeof(Color), typeof(Brush))]
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((targetType == typeof(Brush)) && (value is Color))
            {
                return new SolidColorBrush((Color)value);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
