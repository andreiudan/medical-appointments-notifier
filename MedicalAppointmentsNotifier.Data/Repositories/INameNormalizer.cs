namespace MedicalAppointmentsNotifier.Data.Repositories
{
    public interface INameNormalizer
    {
        public string Normalize(string firstName, string lastName);
    }
}
