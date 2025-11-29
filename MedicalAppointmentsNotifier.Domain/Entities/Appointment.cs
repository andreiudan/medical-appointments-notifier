using MedicalAppointmentsNotifier.Domain.EntityPropertyTypes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalAppointmentsNotifier.Domain.Entities
{
    public class Appointment : BaseEntity
    {
        [Required]
        public MedicalSpecialty? MedicalSpecialty { get; set; }

        [Required]
        public int MonthsInterval { get; set; }

        [Required]
        public AppointmentStatus Status { get; set; }

        [Required]
        public DateTimeOffset? IssuedOn { get; set; }

        public DateTimeOffset? ScheduledOn { get; set; }

        public string ScheduledLocation { get; set; } = string.Empty;

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

            if(!(obj is Appointment))
            {
                return false;
            }

            return Equals(obj as Appointment);
        }

        private bool Equals(Appointment obj)
        {
            return this.Id.CompareTo(obj.Id) == 0 &&
                this.MedicalSpecialty == obj.MedicalSpecialty &&
                this.MonthsInterval == obj.MonthsInterval &&
                this.Status == obj.Status &&
                this.IssuedOn == obj.IssuedOn &&
                this.ScheduledOn == obj.ScheduledOn &&
                this.ScheduledLocation == obj.ScheduledLocation &&
                this.UserId.CompareTo(obj.UserId) == 0;
        }
        public override int GetHashCode()
        {
            HashCode hash = new HashCode();

            hash.Add(this.Id);
            hash.Add(this.MedicalSpecialty);
            hash.Add(this.MonthsInterval);
            hash.Add(this.Status);
            hash.Add(this.IssuedOn);
            hash.Add(this.ScheduledOn);
            hash.Add(this.ScheduledLocation);
            hash.Add(this.UserId);

            return hash.ToHashCode();
        }
    }
}
