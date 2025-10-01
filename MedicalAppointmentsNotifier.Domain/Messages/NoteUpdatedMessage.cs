using MedicalAppointmentsNotifier.Domain.Models;

namespace MedicalAppointmentsNotifier.Domain.Messages
{
    public class NoteUpdatedMessage
    {
        public readonly NoteModel note;

        public NoteUpdatedMessage(NoteModel note)
        {
            this.note = note ?? throw new ArgumentNullException(nameof(note));
        }
    }
}
