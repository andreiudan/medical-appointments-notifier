using MedicalAppointmentsNotifier.Domain.EntityPropertyTypes;

namespace MedicalAppointmentsNotifier.Domain.Models
{
    public class AppointmentModel
    {
        public Guid Id { get; set; }

        public MedicalSpecialty? MedicalSpecialty { get; set; }

        public int IntervalDays { get; set; }

        public AppointmentStatus Status { get; set; }

        public int DaysUntilNextAppointment { get; set; }

        public DateTimeOffset? LatestDate { get; set; }

        public DateTimeOffset? NextDate { get; set; }

        public bool IsSelected { get; set; } = false;

        public override bool Equals(object? obj)
        {
            if(obj is null)
            {
                return false;
            }
            if (obj is not AppointmentModel)
            {
                return false;
            }
            return Equals(obj as AppointmentModel);
        }

        private bool Equals(AppointmentModel obj)
        {
            return this.Id.CompareTo(obj.Id) == 0 &&
                this.MedicalSpecialty == obj.MedicalSpecialty &&
                this.IntervalDays == obj.IntervalDays &&
                this.Status == obj.Status &&
                this.DaysUntilNextAppointment == obj.DaysUntilNextAppointment &&
                this.LatestDate == obj.LatestDate &&
                this.NextDate == obj.NextDate &&
                this.IsSelected == obj.IsSelected;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, MedicalSpecialty, IntervalDays, Status, DaysUntilNextAppointment, LatestDate, NextDate, IsSelected);
        }
    }
}
