using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.EntityPropertyTypes;

namespace MedicalAppointmentsNotifier.Core.Models
{
    public class AppointmentModel
    {
        public Guid Id { get; set; }

        public MedicalSpecialty? MedicalSpecialty { get; set; }

        public int IntervalDays { get; set; }

        public string Status { get; set; }

        public DateTimeOffset? LatestDate { get; set; }

        public DateTimeOffset? NextDate { get; set; }

        public bool IsSelected { get; set; } = false;

        public AppointmentModel(Appointment appointment, bool isSelected = false)
        {
            ArgumentNullException.ThrowIfNull(appointment);

            Id = appointment.Id;
            MedicalSpecialty = appointment.MedicalSpecialty;
            IntervalDays = appointment.IntervalDays;
            Status = appointment.Status;
            LatestDate = appointment.LatestDate;
            NextDate = appointment.NextDate;

            IsSelected = isSelected;
        }

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
            return this.Id.Equals(obj.Id) &&
                this.MedicalSpecialty == obj.MedicalSpecialty &&
                this.IntervalDays == obj.IntervalDays &&
                this.Status == obj.Status &&
                this.LatestDate == obj.LatestDate &&
                this.NextDate == obj.NextDate &&
                this.IsSelected == obj.IsSelected;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, MedicalSpecialty, IntervalDays, Status, LatestDate, NextDate, IsSelected);
        }
    }
}
