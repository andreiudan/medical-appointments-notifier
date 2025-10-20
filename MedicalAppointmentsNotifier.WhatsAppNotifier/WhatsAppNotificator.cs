using MedicalAppointmentsNotifier.Domain.Interfaces;

namespace MedicalAppointmentsNotifier.WhatsAppNotifier
{
    public class WhatsAppNotificator : INotifier
    {
        public Task Notify(string message)
        {
            throw new NotImplementedException();
        }
    }
}
