namespace MedicalAppointmentsNotifier.Domain.Models
{
    public class UserModel
    {
        public Guid Id { get; set; } = new();

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public int DaysUntilNextAppointment { get; set; } = 0;

        public string Status { get; set; } = string.Empty;

        public bool IsSelected { get; set; } = false;

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
                this.DaysUntilNextAppointment == obj.DaysUntilNextAppointment &&
                this.Status == obj.Status;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, FirstName, LastName, DaysUntilNextAppointment, Status);
        }
    }
}
