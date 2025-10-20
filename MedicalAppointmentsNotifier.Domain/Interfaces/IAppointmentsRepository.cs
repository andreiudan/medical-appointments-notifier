using MedicalAppointmentsNotifier.Domain.Entities;

namespace MedicalAppointmentsNotifier.Domain.Interfaces
{
    public interface IAppointmentsRepository : IRepository<Appointment>
    {
        public Task<List<Appointment>> GetExpiringAppointments();
    }
}
