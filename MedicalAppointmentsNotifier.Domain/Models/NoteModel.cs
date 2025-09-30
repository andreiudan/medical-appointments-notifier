namespace MedicalAppointmentsNotifier.Domain.Models
{
    public class NoteModel
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public DateTimeOffset? From { get; set; }

        public DateTimeOffset? Until { get; set; }

        public bool IsSelected { get; set; } = false;

        public override bool Equals(object? obj)
        {
            if(obj is null)
            {
                return false;
            }

            if (obj is not NoteModel)
            {
                return false;
            }

            return Equals(obj as NoteModel);
        }

        private bool Equals(NoteModel obj)
        {
            return this.Id.Equals(obj.Id) &&
                this.Description == obj.Description &&
                this.From == obj.From &&
                this.Until == obj.Until &&
                this.IsSelected == obj.IsSelected;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Description, From, Until, IsSelected);
        }
    }
}
