using MedicalAppointmentsNotifier.Domain.Interfaces;

namespace MedicalAppointmentsNotifier.ReminderJob.Notifiers
{
    internal class EmailNotifierDecorator : BaseNotifierDecorator, INotifier
    {
        public EmailNotifierDecorator(INotifier notifier) : base(notifier)
        {
        }

        public override async Task Notify(string message)
        {
            await base.Notify(message);
        }
    }
}
