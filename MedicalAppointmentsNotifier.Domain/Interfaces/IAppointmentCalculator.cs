namespace MedicalAppointmentsNotifier.Domain.Interfaces
{
    public interface IAppointmentCalculator
    {
        public int CalculateRemainingDays(DateTimeOffset? nextDate);
        public Task<int> CalculateDaysUntilNextAppointmentAsync(Guid userId);
    }
}
