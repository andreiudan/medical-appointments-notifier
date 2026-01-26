using MedicalAppointmentsNotifier.Domain.Models;

namespace MedicalAppointmentsNotifier.Domain.Messages
{
    public class NoteAddedMessage
    {
        public readonly NoteModel note;

        public NoteAddedMessage(NoteModel note)
        {
            this.note = note ?? throw new ArgumentNullException(nameof(note));
        }
    }
}
