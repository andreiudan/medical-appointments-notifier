namespace MedicalAppointmentsNotifier.Domain
{
    public class ValidationConstants
    {
        public const string NameRegex = "^[A-Za-zĂÂÎȘȚăâîșț\\s-]+$";
        public const string NameErrorMessage = "Numele poate contine doar litere, spatii si linii de separare.";

        public const string RequiredErrorMessage = "Campul nu poate fi gol.";
        public const string DatesRequiredErrorMessage = "Atat data de inceput cat si data de sfarsit trebuie completate.";

        public const string DateIntervalErrorMessage = "Data de inceput trebuie sa fie inaintea datei de sfarsit.";

        public const string DaysIntervalErrorMessage = "Intervalul trebuie sa fie un numar pozitiv.";
    }
}
