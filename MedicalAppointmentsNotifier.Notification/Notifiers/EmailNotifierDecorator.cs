using MedicalAppointmentsNotifier.Domain.Interfaces;
using Microsoft.Toolkit.Uwp.Notifications;

namespace MedicalAppointmentsNotifier.ReminderJob.Notifiers
{
    internal class EmailNotifierDecorator : BaseNotifierDecorator, INotifier
    {
        public EmailNotifierDecorator(INotifier notifier) : base(notifier)
        {
        }

        public override Task Notify(string message)
        {
            base.Notify(message);

            new ToastContentBuilder()
                    .AddHeader("AppointmentsNotifier", "Urmatoarele scrisori medicale expira in curand", "")
                    .AddText("Email")
                    .SetToastScenario(ToastScenario.Reminder)
                    .AddButton(new ToastButton()
                        .SetContent("Inchide")
                        .SetDismissActivation()
                        .SetBackgroundActivation())
                    .Show();

            return Task.CompletedTask;
        }
    }
}
