using MedicalAppointmentsNotifier.Domain.EntityPropertyTypes;

namespace MedicalAppointmentsNotifier.Domain.Models
{
    public class AppointmentModel
    {
        public Guid Id { get; set; }

        public MedicalSpecialty? MedicalSpecialty { get; set; }

        public int MonthsInterval { get; set; }

        public AppointmentStatus Status { get; set; }

        public DateTimeOffset? IssuedOn { get; set; }

        public DateTimeOffset? ScheduledOn { get; set; }

        public string ScheduledLocation { get; set; }

        public bool IsSelected { get; set; } = false;

        public int DaysUntilExpiry
        {
            get
            {
                return GetRemainingDaysUntilExpiry();
            }
        }

        public int DaysUntilScheduled
        {
            get
            {
                return GetRemainingDaysUntilScheduled();
            }
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
            return this.Id.CompareTo(obj.Id) == 0 &&
                this.MedicalSpecialty == obj.MedicalSpecialty &&
                this.MonthsInterval == obj.MonthsInterval &&
                this.Status == obj.Status &&
                this.IssuedOn == obj.IssuedOn &&
                this.ScheduledOn == obj.ScheduledOn &&
                this.ScheduledLocation == obj.ScheduledLocation &&
                this.IsSelected == obj.IsSelected;
        }

        public override int GetHashCode()
        {


            return HashCode.Combine(Id, MedicalSpecialty, MonthsInterval, Status, IssuedOn, ScheduledOn, ScheduledLocation, IsSelected);
        }

        public int GetRemainingDaysUntilScheduled()
        {
            if (ScheduledOn.HasValue)
            {
                var remainingTimeSpan = ScheduledOn.Value.Date - DateTimeOffset.Now.Date;
                return (int)remainingTimeSpan.TotalDays <= 0 ? 0 : (int)remainingTimeSpan.TotalDays;
            }
            return GetRemainingDaysUntilExpiry();
        }

        public int GetRemainingDaysUntilExpiry()
        {
            if (IssuedOn.HasValue)
            {
                var expiryDate = IssuedOn.Value.AddMonths(MonthsInterval);
                var remainingTimeSpan = expiryDate.Date - DateTimeOffset.Now.Date;
                return (int)remainingTimeSpan.TotalDays <= 0 ? 0 : (int)remainingTimeSpan.TotalDays;
            }
            return int.MaxValue;
        }
    }
}
