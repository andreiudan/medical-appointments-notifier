using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using System.Text;

namespace MedicalAppointmentsNotifier.Core.Services
{
    public class AppointmentScanner : IAppointmentScanner
    {
        private readonly IAppointmentsRepository repository;

        public AppointmentScanner(IAppointmentsRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<string> GetExpiredAppointmentsMessage()
        {
            string message = await GetNotificationMessage();

            if (string.IsNullOrEmpty(message))
            {
                return string.Empty;
            }

            return message;
        }

        private async Task<string> GetNotificationMessage()
        {
            List<Appointment> appointments = await repository.GetExpiringAppointments();
            if (!appointments.Any())
            {
                return string.Empty;
            }
            appointments = appointments.OrderBy(a => a.NextDate).ToList();

            StringBuilder message = new StringBuilder();

            for (int i = 0; i < appointments.Count(); i++)
            {
                message.AppendLine(string.Format("{0}.{1} {2} - {3} - peste {4} zile.",
                    i,
                    appointments[i].User.LastName,
                    appointments[i].User.FirstName,
                    appointments[i].MedicalSpecialty,
                    (appointments[i].NextDate - DateTime.Now).Value.Days));
            }

            return message.ToString();
        }
    }
}
