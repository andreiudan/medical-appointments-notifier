using System.ComponentModel.DataAnnotations;

namespace MedicalAppointmentsNotifier.Domain.Entities
{
    public class User : BaseEntity
    {
        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = [];

        public ICollection<Note> Notes { get; set; } = [];

        public override bool Equals(object? obj)
        {
            if(obj is null)
            {
                return false;
            }

            if(!(obj is User))
            {
                return false;
            }

            return Equals(obj as User);
        }

        private bool Equals(User obj)
        {
            return this.Id.CompareTo(obj.Id) == 0 &&
                this.FirstName == obj.FirstName &&
                this.LastName == obj.LastName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, FirstName, LastName);
        }
    }
}
