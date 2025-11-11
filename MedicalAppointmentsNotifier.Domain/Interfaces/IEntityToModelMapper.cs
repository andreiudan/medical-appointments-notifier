using MedicalAppointmentsNotifier.Domain.Models;
using MedicalAppointmentsNotifier.Domain.Entities;

namespace MedicalAppointmentsNotifier.Domain.Interfaces
{
    public interface IEntityToModelMapper
    {
        public UserModel Map(User user);

        public NoteModel Map(Note user);

        public AppointmentModel Map(Appointment user);

        public User Map(UserModel userModel);

        public Note Map(NoteModel noteModel, Guid user);

        public Appointment Map(AppointmentModel appointmentModel, Guid user);
    }
}
