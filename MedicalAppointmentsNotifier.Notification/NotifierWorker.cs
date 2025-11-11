using MedicalAppointmentsNotifier.Core.Notificators;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.ReminderJob.Notifiers;

namespace MedicalAppointmentsNotifier.ReminderJob
{
    public class NotifierWorker
    {
        private IAppointmentScanner appointmentScanner;

        public NotifierWorker(IAppointmentScanner appointmentScanner)
        {
            this.appointmentScanner = appointmentScanner ?? throw new ArgumentNullException(nameof(appointmentScanner));
        }

        public async Task RunAsync()
        {
            string message = await appointmentScanner.GetExpiredAppointmentsMessage();

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            var notifier = new TelegramNotifierDecorator(
                                new EmailNotifierDecorator(
                                    new WindowsToastNotifier()));
            await notifier.Notify(message);
        }
    }
}
