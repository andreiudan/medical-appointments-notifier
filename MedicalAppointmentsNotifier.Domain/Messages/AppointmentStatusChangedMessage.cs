using MedicalAppointmentsNotifier.Domain.EntityPropertyTypes;
using MedicalAppointmentsNotifier.Domain.Models;

namespace MedicalAppointmentsNotifier.Domain.Messages
{
    public class AppointmentStatusChangedMessage
    {
        public readonly AppointmentModel appointment;
        public readonly AppointmentStatus oldStatus;
        public readonly AppointmentStatus newStatus;

        public AppointmentStatusChangedMessage(AppointmentModel appointment, AppointmentStatus oldStatus, AppointmentStatus newStatus)
        {
            this.appointment = appointment ?? throw new ArgumentNullException(nameof(appointment));
            this.oldStatus = oldStatus;
            this.newStatus = newStatus;
        }
    }
}
