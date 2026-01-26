using MedicalAppointmentsNotifier.Domain.Models;

namespace MedicalAppointmentsNotifier.Domain.Messages
{
    public class AppointmentAddedMessage
    {
        public readonly AppointmentModel appointment;

        public AppointmentAddedMessage(AppointmentModel appointment)
        {
            this.appointment = appointment ?? throw new ArgumentNullException(nameof(appointment));
        }
    }
}
