using MedicalAppointmentsNotifier.Domain.Interfaces;

namespace MedicalAppointmentsNotifier.Core.Notificators
{
    public class TelegramNotifier : INotifier
    {
        public Task Notify(string message)
        {
            throw new NotImplementedException();
        }
    }
}
