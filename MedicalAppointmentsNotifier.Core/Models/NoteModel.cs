using MedicalAppointmentsNotifier.Domain.Entities;

namespace MedicalAppointmentsNotifier.Core.Models
{
    public class NoteModel
    {
        public Note Note { get; set; } = new();

        public bool IsSelected { get; set; } = false;

        public NoteModel(Note note, bool isSelected)
        {
            Note = note ?? throw new ArgumentNullException(nameof(note));
            IsSelected = isSelected;
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
            return this.Note.Equals(obj.Note) &&
                this.IsSelected == obj.IsSelected;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Note, IsSelected);
        }
    }
}
