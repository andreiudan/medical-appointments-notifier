using System.ComponentModel.DataAnnotations;

namespace MedicalAppointmentsNotifier.Domain.Entities
{
    public class User : BaseEntity
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public List<Appointment> Appointments { get; set; } = new List<Appointment>();

        public List<Note> Notes { get; set; } = new List<Note>();

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
                this.LastName == obj.LastName &&
                this.Appointments.Equals(obj.Appointments) &&
                this.Notes.Equals(obj.Notes);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, FirstName, LastName, Appointments, Notes);
        }
    }
}
