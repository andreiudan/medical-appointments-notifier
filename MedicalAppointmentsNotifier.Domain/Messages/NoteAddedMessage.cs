using MedicalAppointmentsNotifier.Domain.Entities;

namespace MedicalAppointmentsNotifier.Domain.Messages
{
    public class NoteAddedMessage
    {
        public readonly Note note;

        public NoteAddedMessage(Note note)
        {
            this.note = note ?? throw new ArgumentNullException(nameof(note));
        }
    }
}
