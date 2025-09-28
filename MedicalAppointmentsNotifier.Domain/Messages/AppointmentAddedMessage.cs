using MedicalAppointmentsNotifier.Domain.Entities;

namespace MedicalAppointmentsNotifier.Domain.Messages
{
    public class AppointmentAddedMessage
    {
        public readonly Appointment appointment;

        public AppointmentAddedMessage(Appointment appointment)
        {
            this.appointment = appointment ?? throw new ArgumentNullException(nameof(appointment));
        }
    }
}
