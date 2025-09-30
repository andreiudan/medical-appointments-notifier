using MedicalAppointmentsNotifier.Domain.Models;
using MedicalAppointmentsNotifier.Domain.Entities;

namespace MedicalAppointmentsNotifier.Domain.Interfaces
{
    public interface IEntityToModelMapper
    {
        public UserModel Map(User user);

        public NoteModel Map(Note user);

        public AppointmentModel Map(Appointment user);
    }
}
