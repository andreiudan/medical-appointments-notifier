using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalAppointmentsNotifier.Domain.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

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
                this.Name == obj.Name &&
                this.Appointments.Equals(obj.Appointments) &&
                this.Notes.Equals(obj.Notes);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Appointments, Notes);
        }
    }
}
