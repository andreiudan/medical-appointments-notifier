using MedicalAppointmentsNotifier.Domain.Interfaces;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Text;

namespace MedicalAppointmentsNotifier.ReminderJob.Notifiers
{
    internal class WindowsToastNotificator : INotifier
    {
        public Task Notify(string message)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                string[] appointments = message.Trim().Split("\n");

                if(appointments.Length > 6)
                {
                    for(int i = 0; i < 5; i++)
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

                message = stringBuilder.ToString();

                new ToastContentBuilder()
                    .AddHeader("AppointmentsNotifier", "Urmatoarele scrisori medicale expira in curand", "")
                    .AddText(message)
                    .SetToastScenario(ToastScenario.Reminder)
                    .AddButton(new ToastButton()
                        .SetContent("Close")
                        .SetBackgroundActivation())
                    .Show(toast =>
                    {
                        //toast.Activated += (e, s) => { e.Cl}
                    });

                return Task.CompletedTask;
            }
            catch
            {
                throw;
            }
        }
    }
}
