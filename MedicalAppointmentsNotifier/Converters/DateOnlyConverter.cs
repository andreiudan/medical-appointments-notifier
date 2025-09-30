using Microsoft.UI.Xaml.Data;
using System;

namespace MedicalAppointmentsNotifier.Converters
{
    internal class DateOnlyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is DateTimeOffset dateTimeOffset)
            {
                return dateTimeOffset.ToString("dd/MM/yyyy");
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
