using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace MedicalAppointmentsNotifier.Converters
{
    internal class NullablePropertyToVisibilityConverter : IValueConverter
    {
        public bool IsReversed { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool visible = true;

            if (value is null)
            {
                visible = false;
            }

            if(!IsReversed)
            {
                visible = !visible;
            }

            if (visible)
            {
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
