using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;

namespace MedicalAppointmentsNotifier.Core.Services
{
    public class AppointmentCalculator : IAppointmentCalculator
    {
        private readonly IRepository<Appointment> repository;

        public AppointmentCalculator(IRepository<Appointment> repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public int CalculateRemainingDays(DateTimeOffset? nextDate)
        {
            if (!nextDate.HasValue)
            {
                return 0;
            }

            int remainingDays = (nextDate.Value - DateTimeOffset.Now).Days;

            return remainingDays < 0 ? 0 : remainingDays;
        }

        public async Task<int> CalculateDaysUntilNextAppointmentAsync(Guid userId)
        {
            IEnumerable<Appointment> appointments = await repository.FindAllAsync(a => a.User.Id == userId);
            DateTimeOffset? nextAppointment = appointments.OrderByDescending(a => a.NextDate).FirstOrDefault()?.NextDate;

            int daysUntilNextAppointment = nextAppointment.HasValue
                ? (nextAppointment.Value - DateTimeOffset.Now).Days
                : 0;

            return daysUntilNextAppointment;
        }
    }
}
