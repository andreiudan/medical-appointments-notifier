using MedicalAppointmentsNotifier.Domain.Entities;

namespace MedicalAppointmentsNotifier.Core.Models
{
    public class UserModel
    {
        public Guid Id { get; set; } = new();

        public string Name { get; set; } = string.Empty;

        public int DaysUntilNextAppointment { get; set; } = 0;

        public string Status { get; set; } = string.Empty;

        public bool IsSelected { get; set; } = false;

        public UserModel(User user, int daysUntilNextAppointment = 0, bool isSelected = false)
        {
            ArgumentNullException.ThrowIfNull(user);

            Id = user.Id;
            Name = user.Name;

            DaysUntilNextAppointment = daysUntilNextAppointment;
            IsSelected = isSelected;
        }

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
            return this.Id.Equals(obj.Id) &&
                this.Name == obj.Name &&
                this.DaysUntilNextAppointment == obj.DaysUntilNextAppointment &&
                this.Status == obj.Status &&
                this.IsSelected == obj.IsSelected;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, DaysUntilNextAppointment, Status, IsSelected);
        }
    }
}
