using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text;

namespace MedicalAppointmentsNotifier.Core.Services
{
    public class AppointmentScanner : IAppointmentScanner
    {
        private readonly IAppointmentsRepository repository;
        private readonly ILogger<AppointmentScanner> logger;

        public AppointmentScanner(IAppointmentsRepository repository, ILogger<AppointmentScanner> logger)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> GetExpiredAppointmentsMessage()
        {
            string message = await GetNotificationMessage();

            if (string.IsNullOrEmpty(message))
            {
                logger.LogInformation("No expiring appointments found.");
                return string.Empty;
            }

            return message;
        }

        private async Task<string> GetNotificationMessage()
        {
            List<Appointment> appointments = await repository.GetAllExpiringAppointments();
            if (!appointments.Any())
            {
                return string.Empty;
            }
            appointments = appointments.OrderBy(a => a.IssuedOn.Value.AddMonths(a.MonthsInterval)).ToList();

            StringBuilder message = new StringBuilder();

            for (int i = 0; i < appointments.Count(); i++)
            {
                message.AppendLine(string.Format("{0}.{1} {2} - {3} - peste {4} zile.",
                    i,
                    appointments[i].User.LastName,
                    appointments[i].User.FirstName,
                    appointments[i].MedicalSpecialty,
                    appointments[i].IssuedOn.Value.AddMonths(appointments[i].MonthsInterval).Subtract(DateTimeOffset.Now).Days));
            }

            logger.LogInformation("{Count} expiring appointments found", appointments.Count());
            return message.ToString();
        }
    }
}
