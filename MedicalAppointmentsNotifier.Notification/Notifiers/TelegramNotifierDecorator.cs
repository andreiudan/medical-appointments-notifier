using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.ReminderJob.Notifiers;

namespace MedicalAppointmentsNotifier.Core.Notificators
{
    internal class TelegramNotifierDecorator : BaseNotifierDecorator, INotifier
    {
        public TelegramNotifierDecorator(INotifier notifier) : base(notifier)
        {
        }

        public override async Task Notify(string message)
        {
            await base.Notify(message);
        }
    }
}
