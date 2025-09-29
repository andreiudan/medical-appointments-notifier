using MedicalAppointmentsNotifier.Domain.Entities;

namespace MedicalAppointmentsNotifier.Core.Models
{
    public class UserModel
    {
        public User User { get; set; } = new();

        public bool IsSelected { get; set; } = false;

        public UserModel(User user, bool isSelected)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
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
            return this.User.Equals(obj.User) &&
                this.IsSelected == obj.IsSelected;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(User, IsSelected);
        }
    }
}
