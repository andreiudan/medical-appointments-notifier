using CommunityToolkit.Mvvm.ComponentModel;

namespace MedicalAppointmentsNotifier.Domain.Models
{
    public partial class UserModel : ObservableObject
    {
        public Guid Id { get; set; } = new();

        [ObservableProperty]
        public partial string FirstName { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string LastName { get; set; } = string.Empty;

        [ObservableProperty]
        public partial int UpcominAppointmentsCount { get; set; } = 0;

        [ObservableProperty]
        public partial int ExpiringAppointmentsCount { get; set; } = 0;

        public override bool Equals(object? obj)
        {
            if(obj is null)
            {
                return false;
            }

            if (obj is not UserModel)
            {
                return false;
            }

            return Equals(obj as UserModel);
        }

        private bool Equals(UserModel obj)
        {
            return this.Id.CompareTo(obj.Id) == 0 &&
                this.FirstName == obj.FirstName &&
                this.LastName == obj.LastName &&
                this.UpcominAppointmentsCount == obj.UpcominAppointmentsCount &&
                this.ExpiringAppointmentsCount == obj.ExpiringAppointmentsCount;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, FirstName, LastName, UpcominAppointmentsCount, ExpiringAppointmentsCount);
        }
    }
}
