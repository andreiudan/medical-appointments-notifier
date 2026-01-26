using MedicalAppointmentsNotifier.Domain.EntityPropertyTypes;
using MedicalAppointmentsNotifier.Domain.Models;

namespace MedicalAppointmentsNotifier.Domain.Messages
{
    public class AppointmentStatusChangedMessage
    {
        public readonly AppointmentModel appointment;
        public readonly AppointmentStatus oldStatus;

        public AppointmentStatusChangedMessage(AppointmentModel appointment, AppointmentStatus oldStatus)
        {
            this.appointment = appointment ?? throw new ArgumentNullException(nameof(appointment));
            this.oldStatus = oldStatus;
        }
    }
}
