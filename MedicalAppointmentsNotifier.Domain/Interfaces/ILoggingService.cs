namespace MedicalAppointmentsNotifier.Domain.Interfaces
{
    public interface ILoggingService
    {
        public void LogInformation(string messageTemplate, params object[] propertyValues);

        public void LogError(Exception exception, string messageTemplate, params object[] propertyValues);

        public void LogWarning(string messageTemplate, params object[] propertyValues);

        public void LogDebug(string messageTemplate, params object[] propertyValues);

        public void LogFatal(Exception exception, string messageTemplate, params object[] propertyValues);
    }
}
