using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalAppointmentsNotifier.Domain.Entities
{
    public class Note : BaseEntity
    {
        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public DateTimeOffset? From { get; set; }

        [Required]
        public int MonthsPeriod { get; set; }

        [Required]
        [ForeignKey("User")]
        public Guid UserId { get; set; }

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
                this.Title == obj.Title &&
                this.Description == obj.Description &&
                this.From == obj.From &&
                this.MonthsPeriod == obj.MonthsPeriod &&
                this.UserId.CompareTo(obj.UserId) == 0;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Title, Description, From, MonthsPeriod, UserId);
        }
    }
}
