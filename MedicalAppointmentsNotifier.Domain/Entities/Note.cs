using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalAppointmentsNotifier.Domain.Entities
{
    public class Note
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime From { get; set; }

        [Required]
        public DateTime Until { get; set; }

        [Required]
        public User? User { get; set; }

        public override bool Equals(object? obj)
        {
            if(obj is null)
            {
                return false;
            }

            if(!(obj is Note))
            {
                return false;
            }

            return Equals(obj as Note);
        }

        private bool Equals(Note obj)
        {
            return this.Id.CompareTo(obj.Id) == 0 &&
                this.Description == obj.Description &&
                this.From == obj.From &&
                this.Until == obj.Until &&
                this.User.Equals(obj.User);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Description, From, Until, User);
        }
    }
}
