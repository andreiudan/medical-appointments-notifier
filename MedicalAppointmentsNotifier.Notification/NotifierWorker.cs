using MedicalAppointmentsNotifier.Core.Notificators;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.ReminderJob.Notifiers;
using Microsoft.Extensions.Logging;

namespace MedicalAppointmentsNotifier.ReminderJob
{
    public class NotifierWorker
    {
        private readonly IAppointmentScanner appointmentScanner;
        private readonly ILogger<NotifierWorker> logger;

        public NotifierWorker(IAppointmentScanner appointmentScanner, ILogger<NotifierWorker> logger)
        {
            this.appointmentScanner = appointmentScanner ?? throw new ArgumentNullException(nameof(appointmentScanner));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task RunAsync()
        {
            try
            {
                logger.LogInformation("Starting appointment scan for expired appointments.");

                string message = await appointmentScanner.GetExpiredAppointmentsMessage();

                if (string.IsNullOrEmpty(message))
                {
                    return;
                }

                logger.LogInformation("Sending expired appointments notification.");

                var notifier = new TelegramNotifierDecorator(
                                    new EmailNotifierDecorator(
                                        new WindowsToastNotifier()));
                await notifier.Notify(message);

                logger.LogInformation("Expired appointments notification sent.");
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "An error occurred while running the expired appointments notifier worker.");
            }
        }
    }
}
