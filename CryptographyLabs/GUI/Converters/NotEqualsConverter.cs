using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace CryptographyLabs.GUI
{
    class NotEqualsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            bool res = !value.Equals(parameter);
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
