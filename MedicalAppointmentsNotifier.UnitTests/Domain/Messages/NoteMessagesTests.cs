using MedicalAppointmentsNotifier.Domain.Messages;
using MedicalAppointmentsNotifier.Domain.Models;

namespace MedicalAppointmentsNotifier.UnitTests.Domain.Messages
{
    [TestFixture]
    public class NoteMessagesTests
    {
        private NoteModel noteFake;

        [SetUp]
        public void SetUp()
        {
            noteFake = new NoteModel()
            {
                Id = Guid.NewGuid(),
                Description = "description",
                From = DateTime.Now
            };
        }

        [Test]
        public void AddedConstructor_WhenNoteIsNull_ThrowsArgumentNullException()
        {
            //Assert
            Assert.Throws<ArgumentNullException>(() => new NoteAddedMessage(null));
        }

        [Test]
        public void AddedConstructor_Always_SetsNoteField()
        {
            //Act
            NoteAddedMessage message = new NoteAddedMessage(noteFake);

            //Assert
            Assert.That(message.note, Is.EqualTo(noteFake));
        }
    }
}
