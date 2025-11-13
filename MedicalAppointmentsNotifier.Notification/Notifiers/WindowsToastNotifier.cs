using MedicalAppointmentsNotifier.Domain.Interfaces;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Text;

namespace MedicalAppointmentsNotifier.ReminderJob.Notifiers
{
    internal class WindowsToastNotifier : INotifier
    {
        public Task Notify(string message)
        {
            try
            {
                message = TrimMessageLength(message);

                new ToastContentBuilder()
                   .AddText("Urmatoarele scrisori medicale expira in curand",
                            hintStyle: AdaptiveTextStyle.Title)
                   .AddText(message,
                            hintStyle: AdaptiveTextStyle.Body)
                   .SetToastScenario(ToastScenario.Reminder)
                   .AddToastActivationInfo("closeApp", ToastActivationType.Background)
                   .AddButton(new ToastButton()
                        .SetContent("Inchide")
                        .SetDismissActivation())
                   .Show();

                return Task.CompletedTask;
            }
            catch
            {
                throw;
            }
        }

        private string TrimMessageLength(string message)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string[] appointments = message.Trim().Split("\n");

            if (appointments.Length > 6)
            {
                for (int i = 0; i < 5; i++)
                {
                    stringBuilder.Append(appointments[i]);
                }
                stringBuilder.Append(string.Format("+{0} scrisori medicale care urmeaza sa expire", (appointments.Length - 5)));
            }
            else
            {
                for (int i = 0; i < appointments.Length; i++)
                {
                    stringBuilder.Append(appointments[i]);
                }
            }

            return stringBuilder.ToString();
        }
    }
}
