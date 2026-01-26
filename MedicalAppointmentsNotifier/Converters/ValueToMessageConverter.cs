using Microsoft.UI.Xaml.Data;
using System;

namespace MedicalAppointmentsNotifier.Converters
{
    class ValueToMessageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return string.Empty;
            }

            // Attempt to parse numeric value (works for int, long, double, etc.)
            if (!double.TryParse(value.ToString(), out var numeric))
            {
                // If it's not numeric, just return the original value as string
                return value.ToString();
            }

            // If value is below zero, return custom expired message
            if (numeric < 0)
            {
                return "Expirat";
            }

            // Use converter parameter as a format string, fallback to a default
            var format = parameter as string;
            if (!string.IsNullOrWhiteSpace(format))
            {
                try
                {
                    // If you want to show integer days, cast to int
                    return string.Format(format, (int)Math.Round(numeric));
                }
                catch
                {
                    // If formatting fails, fall back to a simple representation
                    return numeric.ToString();
                }
            }

            return numeric.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
