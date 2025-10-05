using MedicalAppointmentsNotifier.Domain.Entities;

namespace MedicalAppointmentsNotifier.UnitTests.Domain.Entities
{
    [TestFixture]
    public class NoteTests
    {
        private Note noteFake;
        private Note differentNote;
        private Note sameNote;

        [SetUp]
        public void SetUp()
        {
            var user = new User { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };

            noteFake = new Note
            {
                Id = Guid.NewGuid(),
                Description = "Follow-up visit",
                From = DateTimeOffset.Now,
                Until = DateTimeOffset.Now.AddDays(2),
                User = user
            };

            differentNote = new Note
            {
                Id = Guid.NewGuid(),
                Description = "Different note",
                From = DateTimeOffset.Now.AddDays(-5),
                Until = DateTimeOffset.Now.AddDays(-3),
                User = new User { Id = Guid.NewGuid(), FirstName = "Alex", LastName = "Bean" }
            };

            sameNote = new Note
            {
                Id = noteFake.Id,
                Description = "Follow-up visit",
                From = noteFake.From,
                Until = noteFake.Until,
                User = user
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
        public void Equals_ParameterIsNotNote_ReturnsFalse()
        {
            bool result = noteFake.Equals(new Appointment());
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
