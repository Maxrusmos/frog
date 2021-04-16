using System;
using System.Text;
using System.Windows.Data;

namespace CryptographyLabs.GUI
{
    [ValueConversion(typeof(bool), typeof(bool))]
    class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value is bool boolValue)
                return !boolValue;
            else
                throw new InvalidOperationException("Received not bool value in boolean converter.");
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
