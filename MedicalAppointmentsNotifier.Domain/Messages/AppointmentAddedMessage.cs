using MedicalAppointmentsNotifier.Domain.Entities;

namespace MedicalAppointmentsNotifier.Domain.Messages
{
    public class AppontmentAddedMessage
    {
        public readonly Appointment appointment;

        public AppontmentAddedMessage(Appointment appointment)
        {
            this.appointment = appointment ?? throw new ArgumentNullException(nameof(appointment));
        }
    }
}
