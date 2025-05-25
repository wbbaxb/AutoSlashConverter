using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AutoSlashConverter.Presentation.Converters
{
    public class CountToVisibilityConverter : IValueConverter
    {
        public bool IsReversed { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                if (IsReversed)
                {
                    return count == 0 ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    return count == 0 ? Visibility.Collapsed : Visibility.Visible;
                }
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}