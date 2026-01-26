namespace MedicalAppointmentsNotifier.Domain.Interfaces
{
    public interface IAppointmentCalculator
    {
        public int CalculateRemainingDays(DateTimeOffset? nextDate);
    }
}
