using MedicalAppointmentsNotifier.Domain.Entities;

namespace MedicalAppointmentsNotifier.Core.Models
{
    public class AppointmentModel
    {
        public Appointment Appointment { get; set; } = new();

        public bool IsSelected { get; set; } = false;

        public AppointmentModel(Appointment appointment, bool isSelected)
        {
            Appointment = appointment ?? throw new ArgumentNullException(nameof(appointment));
            IsSelected = isSelected;
        }

        public override bool Equals(object? obj)
        {
            if(obj is null)
            {
                return false;
            }
            if (obj is not AppointmentModel)
            {
                return false;
            }
            return Equals(obj as AppointmentModel);
        }

        private bool Equals(AppointmentModel obj)
        {
            return this.Appointment.Equals(obj.Appointment) &&
                this.IsSelected == obj.IsSelected;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Appointment, IsSelected);
        }
    }
}
