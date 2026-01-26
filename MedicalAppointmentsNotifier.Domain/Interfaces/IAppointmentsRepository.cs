using MedicalAppointmentsNotifier.Domain.Entities;

namespace MedicalAppointmentsNotifier.Domain.Interfaces
{
    public interface IAppointmentsRepository : IRepository<Appointment>
    {
        public Task<List<Appointment>> GetAllExpiringAppointments();

        public Task<List<Appointment>> GetExpiringAppointments(Guid userId);

        public Task<List<Appointment>> GetUpcomingAppointments(Guid userId);

        public Task<List<Appointment>> GetPastAppointments(Guid userId);

        public Task<int> GetExpiringAppointmentsCount(Guid userId);

        public Task<int> GetUpcomingAppointmentsCount(Guid userId);
    }
}
