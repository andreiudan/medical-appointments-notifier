using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.ReminderJob.Notifiers;
using Microsoft.Toolkit.Uwp.Notifications;

namespace MedicalAppointmentsNotifier.Core.Notificators
{
    internal class TelegramNotifierDecorator : BaseNotifierDecorator, INotifier
    {
        public TelegramNotifierDecorator(INotifier notifier) : base(notifier)
        {
        }

        public override Task Notify(string message)
        {
            base.Notify(message);

            new ToastContentBuilder()
                    .AddHeader("AppointmentsNotifier", "Urmatoarele scrisori medicale expira in curand", "")
                    .AddText("Telegram")
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
