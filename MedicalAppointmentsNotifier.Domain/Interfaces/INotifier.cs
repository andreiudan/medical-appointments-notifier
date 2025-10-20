namespace MedicalAppointmentsNotifier.Domain.Interfaces
{
    public interface INotifier
    {
        public Task Notify(string message);
    }
}
