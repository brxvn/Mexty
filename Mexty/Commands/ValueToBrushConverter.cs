using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Mexty.Commands {
    public class ValueToBrushConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            int input = (int)value;

            if (input <= 10) {
                return Brushes.Red;
            }
            else if (input > 10 && input <= 20) {
                return Brushes.Blue;
            }
            else {
                return DependencyProperty.UnsetValue;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
}
