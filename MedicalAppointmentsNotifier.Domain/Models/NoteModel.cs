namespace MedicalAppointmentsNotifier.Domain.Models
{
    public class NoteModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTimeOffset? From { get; set; }

        public int MonthsPeriod { get; set; }

        public bool IsSelected { get; set; } = false;

        public DateTimeOffset? Until
        {
            get
            {
                return GetUntilDate();
            }
        }

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
            return this.Id.CompareTo(obj.Id) == 0 &&
                this.Title == obj.Title &&
                this.Description == obj.Description &&
                this.From == obj.From &&
                this.MonthsPeriod == obj.MonthsPeriod &&
                this.IsSelected == obj.IsSelected;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Title, Description, From, MonthsPeriod, IsSelected);
        }

        public DateTimeOffset GetUntilDate()
        {
            if (From.HasValue)
            {
                return From.Value.AddMonths(MonthsPeriod);
            }

            return DateTimeOffset.MinValue;
        }
    }
}
