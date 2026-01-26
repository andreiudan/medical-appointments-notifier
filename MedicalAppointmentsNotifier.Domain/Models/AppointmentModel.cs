using CommunityToolkit.Mvvm.ComponentModel;
using MedicalAppointmentsNotifier.Domain.EntityPropertyTypes;

namespace MedicalAppointmentsNotifier.Domain.Models
{
    public partial class AppointmentModel : ObservableObject
    {
        public Guid Id { get; set; }

        [ObservableProperty]
        public partial MedicalSpecialty? MedicalSpecialty { get; set; } = null;

        [ObservableProperty]
        public partial AppointmentStatus Status { get; set; } = AppointmentStatus.Neprogramat;

        [ObservableProperty]
        public partial int MonthsInterval { get; set; } = 3;

        [ObservableProperty]
        public partial DateTimeOffset? IssuedOn { get; set; } = DateTimeOffset.Now;

        [ObservableProperty]
        public partial DateTimeOffset? ScheduledOn { get; set; } = null;

        [ObservableProperty]
        public partial string? ScheduledLocation { get; set; } = string.Empty;

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
                this.ScheduledLocation == obj.ScheduledLocation;
        }

        public override int GetHashCode()
        {


            return HashCode.Combine(Id, MedicalSpecialty, MonthsInterval, Status, IssuedOn, ScheduledOn, ScheduledLocation);
        }

        public int GetRemainingDaysUntilScheduled()
        {
            if (ScheduledOn.HasValue)
            {
                var remainingTimeSpan = ScheduledOn.Value.Date - DateTimeOffset.Now.Date;
                return (int)remainingTimeSpan.TotalDays;
            }
            return GetRemainingDaysUntilExpiry();
        }

        public int GetRemainingDaysUntilExpiry()
        {
            if (IssuedOn.HasValue)
            {
                var expiryDate = IssuedOn.Value.AddMonths(MonthsInterval);
                var remainingTimeSpan = expiryDate.Date - DateTimeOffset.Now.Date;
                return (int)remainingTimeSpan.TotalDays;
            }
            return int.MaxValue;
        }
    }
}
