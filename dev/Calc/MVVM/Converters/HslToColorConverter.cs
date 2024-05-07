using Calc.Core.Color;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Calc.MVVM.Converters
{
    public class HslToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is HslColor hsl)
            {
                var rgb = CalcColorConverter.HslToRgb(hsl);
                return new SolidColorBrush(Color.FromArgb(255, rgb.R, rgb.G, rgb.B));
            }
            return new SolidColorBrush(Color.FromArgb(50, 40, 120, 78));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
