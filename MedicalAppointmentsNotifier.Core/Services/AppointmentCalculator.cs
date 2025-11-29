using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MedicalAppointmentsNotifier.Core.Services
{
    public class AppointmentCalculator : IAppointmentCalculator
    {
        private readonly IRepository<Appointment> repository;
        private readonly ILogger<AppointmentCalculator> logger;

        public AppointmentCalculator(IRepository<Appointment> repository, ILogger<AppointmentCalculator> logger)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            IEnumerable<Appointment> appointments = await repository.FindAllAsync(a => a.User.Id == userId).ConfigureAwait(false);
            Appointment? nextAppointment = appointments.OrderBy(a => a.IssuedOn.Value.AddMonths(a.MonthsInterval)).FirstOrDefault();

            if (nextAppointment == null)
            {
                logger.LogInformation("No appointments found for user with ID:{UserId}.", userId);
                return 0;
            }

            int daysUntilNextAppointment = nextAppointment.IssuedOn.Value.AddMonths(nextAppointment.MonthsInterval).Subtract(DateTimeOffset.Now).Days;

            logger.LogInformation("User with ID:{UserId} has {DaysUntilNextAppointment} days until next appointment.", userId, daysUntilNextAppointment);
            return daysUntilNextAppointment;
        }
    }
}
