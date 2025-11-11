using MedicalAppointmentsNotifier.Domain.EntityPropertyTypes;

namespace MedicalAppointmentsNotifier.Domain.Models
{
    public class NotificationDetailsModel
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public MedicalSpecialty? MedicalSpecialty { get; set; }

        public int RemainingDays { get; set; } = 0;

        public override bool Equals(object? obj)
        {
            if(obj is null)
            {
                return false;
            }

            if(!(obj is NotificationDetailsModel))
            {
                return false;
            }

            return Equals(obj as NotificationDetailsModel);
        }

        private bool Equals(NotificationDetailsModel other)
        {
            return FirstName == other.FirstName &&
                LastName == other.LastName &&
                MedicalSpecialty == other.MedicalSpecialty &&
                RemainingDays == other.RemainingDays;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FirstName, LastName, MedicalSpecialty, RemainingDays);
        }
    }
}
