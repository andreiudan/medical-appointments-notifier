namespace MedicalAppointmentsNotifier.Domain.Interfaces
{
    public interface INameNormalizer
    {
        public string Normalize(string firstName, string lastName);
    }
}
