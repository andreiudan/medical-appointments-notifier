using MedicalAppointmentsNotifier.Domain.Models;
using Microsoft.UI.Xaml.Data;
using System;

namespace MedicalAppointmentsNotifier.Converters
{
    public class UserFullNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is null || value is not UserModel)
            {
                return string.Empty;
            }

            UserModel user = (UserModel)value;

            return string.Format("{0} {1}", user.LastName, user.FirstName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
