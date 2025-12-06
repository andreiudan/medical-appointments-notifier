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

        public async Task<List<Appointment>> GetAllExpiringAppointments()
        {
            IEnumerable<Appointment> appointments = context.Set<Appointment>()
                .Where(a => (int)a.Status == 0)
                .Include(a => a.User)
                .AsNoTracking()
                .AsEnumerable();

            return appointments.Where(a => a.IssuedOn.Value.AddMonths(a.MonthsInterval) > DateTimeOffset.Now &&
                                           a.IssuedOn.Value.AddMonths(a.MonthsInterval).Subtract(DateTimeOffset.Now).Days <= 30)
                               .ToList();
        }

        public async Task<List<Appointment>> GetExpiringAppointments(Guid userId)
        {
            IEnumerable<Appointment> appointments = context.Set<Appointment>()
                .Where(a => a.UserId.Equals(userId) && (int)a.Status == 0)
                .AsNoTracking()
                .AsEnumerable();

            return appointments.Where(a => a.IssuedOn.Value.AddMonths(a.MonthsInterval) > DateTimeOffset.Now)
                               .OrderByDescending(a => a.IssuedOn.Value.AddMonths(a.MonthsInterval))
                               .ToList();
        }

        public async Task<List<Appointment>> GetUpcomingAppointments(Guid userId)
        {
            IEnumerable<Appointment> appointments = context.Set<Appointment>()
                .Where(a => a.UserId.Equals(userId) && (int)a.Status == 1)
                .AsNoTracking()
                .AsEnumerable();

            return appointments.OrderBy(a =>
                                {
                                    if (a.ScheduledOn.HasValue)
                                    {
                                        return a.ScheduledOn;
                                    }

                                    return a.IssuedOn.Value.AddMonths(a.MonthsInterval);
                                })
                                .ToList();
        }

        public async Task<List<Appointment>> GetPastAppointments(Guid userId)
        {
            IEnumerable<Appointment> appointments = context.Set<Appointment>()
            .Where(a => a.UserId.Equals(userId) && (int)a.Status == 2)
            .AsNoTracking()
            .AsEnumerable();

            return appointments.GroupBy(a => a.MedicalSpecialty)
                .Select(g => g.OrderByDescending(a => a.IssuedOn.Value.AddMonths(a.MonthsInterval)).First())
                .ToList();
        }

        public async Task<int> GetExpiringAppointmentsCount(Guid userId)
        {
            return context.Set<Appointment>()
                          .Count(a => a.UserId.Equals(userId) && 
                                 (int)a.Status == 0);
        }

        public async Task<int> GetUpcomingAppointmentsCount(Guid userId)
        {
            return context.Set<Appointment>()
                          .Count(a => a.UserId.Equals(userId) &&
                                 (int)a.Status == 1);
        }
    }
}
