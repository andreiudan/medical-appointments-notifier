using CommunityToolkit.Mvvm.ComponentModel;

namespace MedicalAppointmentsNotifier.Domain.Models
{
    public partial class NoteModel : ObservableObject
    {
        public Guid Id { get; set; }

        [ObservableProperty]
        public partial string Title { get; set; }

        [ObservableProperty]
        public partial string Description { get; set; }

        [ObservableProperty]
        public partial DateTimeOffset? From { get; set; }

        [ObservableProperty]
        public partial int MonthsPeriod { get; set; }

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
                this.MonthsPeriod == obj.MonthsPeriod;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Title, Description, From, MonthsPeriod);
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
