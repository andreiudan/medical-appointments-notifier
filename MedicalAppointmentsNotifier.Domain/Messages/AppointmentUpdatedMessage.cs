using MedicalAppointmentsNotifier.Domain.Models;

namespace MedicalAppointmentsNotifier.Domain.Messages
{
    public class AppointmentUpdatedMessage
    {
        public AppointmentModel appointment;

        public AppointmentUpdatedMessage(AppointmentModel appointment)
        {
            this.appointment = appointment ?? throw new ArgumentNullException(nameof(appointment));
        }
    }
}
