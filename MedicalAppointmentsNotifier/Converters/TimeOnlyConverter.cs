using Microsoft.UI.Xaml.Data;
using System;

namespace MedicalAppointmentsNotifier.Converters
{
    class TimeOnlyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is DateTimeOffset dateTimeOffset)
            {
                if (dateTimeOffset.ToLocalTime().TimeOfDay.Equals(TimeSpan.Zero))
                {
                    return "Fara ora";
                }
                return dateTimeOffset.ToLocalTime().ToString("t");
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
