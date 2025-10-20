using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.EntityPropertyTypes;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointmentsNotifier.Data.Repositories
{
    public class AppointmentsRepository : Repository<Appointment>, IAppointmentsRepository
    {
        private readonly MedicalAppointmentsContext context;

        public AppointmentsRepository(MedicalAppointmentsContext context) : base(context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Appointment>> GetExpiringAppointments()
        {
            List<Appointment> appointments = context.Set<Appointment>()
                .Where(a => (int)a.Status == 0)
                .Include(a => a.User)
                .AsNoTracking()
                .AsEnumerable()
                .Where(a => a.NextDate > DateTimeOffset.Now &&
                            (a.NextDate - DateTimeOffset.Now).Value.Days <= 30)
                .ToList();

            return appointments;
        }
    }
}
