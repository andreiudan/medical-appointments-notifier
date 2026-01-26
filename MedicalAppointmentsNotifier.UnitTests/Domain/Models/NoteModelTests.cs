using MedicalAppointmentsNotifier.Domain.Models;

namespace MedicalAppointmentsNotifier.UnitTests.Domain.Models
{
    [TestFixture]
    public class NoteModelTests
    {
        private NoteModel noteFake;
        private NoteModel differentNote;
        private NoteModel sameNote;

        [SetUp]
        public void SetUp()
        {
            noteFake = new NoteModel
            {
                Id = Guid.NewGuid(),
                Description = "Follow-up visit",
                From = DateTimeOffset.Now
            };

            differentNote = new NoteModel
            {
                Id = Guid.NewGuid(),
                Description = "Annual checkup",
                From = DateTimeOffset.Now.AddDays(-5)
            };

            sameNote = new NoteModel
            {
                Id = noteFake.Id,
                Description = "Follow-up visit",
                From = noteFake.From,
                MonthsPeriod = noteFake.MonthsPeriod
            };
        }

        [Test]
        public void GetHashCode_SameValues_ReturnsSameHashCode()
        {
            int hash1 = noteFake.GetHashCode();
            int hash2 = sameNote.GetHashCode();

            Assert.That(hash1, Is.EqualTo(hash2));
        }

        [Test]
        public void GetHashCode_DifferentValues_ReturnsDifferentHashCode()
        {
            int hash1 = noteFake.GetHashCode();
            int hash2 = differentNote.GetHashCode();

            Assert.That(hash1, Is.Not.EqualTo(hash2));
        }

        [Test]
        public void GetHashCode_MultipleCalls_ReturnsSameHashCode()
        {
            int hash1 = noteFake.GetHashCode();
            int hash2 = noteFake.GetHashCode();
            int hash3 = noteFake.GetHashCode();

            Assert.Multiple(() =>
            {
                Assert.That(hash1, Is.EqualTo(hash2));
                Assert.That(hash1, Is.EqualTo(hash3));
            });
        }

        [Test]
        public void Equals_ParameterIsNull_ReturnsFalse()
        {
            bool result = noteFake.Equals(null);
            Assert.That(result, Is.False);
        }

        [Test]
        public void Equals_ParameterIsNotNoteModel_ReturnsFalse()
        {
            bool result = noteFake.Equals(new AppointmentModel());
            Assert.That(result, Is.False);
        }

        [Test]
        public void Equals_SameValues_ReturnsTrue()
        {
            bool result = noteFake.Equals(sameNote);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Equals_DifferentValues_ReturnsFalse()
        {
            bool result = noteFake.Equals(differentNote);
            Assert.That(result, Is.False);
        }
    }
}
