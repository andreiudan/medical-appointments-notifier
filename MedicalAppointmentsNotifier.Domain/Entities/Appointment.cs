using MedicalAppointmentsNotifier.Domain.EntityPropertyTypes;
using System.ComponentModel.DataAnnotations;

namespace MedicalAppointmentsNotifier.Domain.Entities
{
    public class Appointment : BaseEntity
    {
        [Required]
        public MedicalSpecialty? MedicalSpecialty { get; set; }

        [Required]
        public int IntervalDays { get; set; }

        [Required]
        public AppointmentStatus Status { get; set; }

        [Required]
        public DateTimeOffset? LatestDate { get; set; }

        [Required]
        public DateTimeOffset? NextDate { get; set; }

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
                this.IntervalDays == obj.IntervalDays &&
                this.Status == obj.Status &&
                this.LatestDate == obj.LatestDate &&
                this.NextDate == obj.NextDate;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, MedicalSpecialty, IntervalDays, Status, LatestDate, NextDate);
        }
    }
}
