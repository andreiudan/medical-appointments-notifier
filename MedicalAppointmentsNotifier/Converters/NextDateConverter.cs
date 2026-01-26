using Microsoft.UI.Xaml.Data;
using System;

namespace MedicalAppointmentsNotifier.Converters
{
    internal class NextDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is DateTimeOffset dateTimeOffset)
            {
                if(parameter is int daysToAdd)
                {
                    DateTimeOffset newDate = dateTimeOffset.AddDays(daysToAdd);
                    return newDate.ToString("dd/MM/yyyy");
                }
            }

            return DateTimeOffset.MinValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
