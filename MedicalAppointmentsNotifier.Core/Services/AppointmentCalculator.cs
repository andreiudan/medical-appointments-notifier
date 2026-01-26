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
    }
}
