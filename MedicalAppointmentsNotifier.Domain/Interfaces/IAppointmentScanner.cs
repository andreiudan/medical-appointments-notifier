namespace MedicalAppointmentsNotifier.Domain.Interfaces
{
    public interface IAppointmentScanner
    {
        public Task<string> GetExpiredAppointmentsMessage();
    }
}
