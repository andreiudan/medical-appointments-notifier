using MedicalAppointmentsNotifier.Domain.Interfaces;

namespace MedicalAppointmentsNotifier.ReminderJob.Notifiers
{
    internal abstract class BaseNotifierDecorator : INotifier
    {
        private readonly INotifier notifier;

        protected BaseNotifierDecorator(INotifier notifier)
        {
            this.notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
        }

        public virtual async Task Notify(string message)
        {
            await notifier.Notify(message);
        }
    }
}
