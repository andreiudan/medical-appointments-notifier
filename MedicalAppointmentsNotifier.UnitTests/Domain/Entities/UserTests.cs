using MedicalAppointmentsNotifier.Domain.Entities;

namespace MedicalAppointmentsNotifier.UnitTests.Domain.Entities
{
    [TestFixture]
    public class UserTests
    {
        private User userFake;
        private User differentUser;
        private User sameUser;

        [SetUp]
        public void SetUp()
        {
            userFake = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Appointments = new List<Appointment>(),
                Notes = new List<Note>()
            };

            differentUser = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Alex",
                LastName = "Bean",
                Appointments = new List<Appointment>(),
                Notes = new List<Note>()
            };

            sameUser = new User
            {
                Id = userFake.Id,
                FirstName = "John",
                LastName = "Doe",
                Appointments = new List<Appointment>(),
                Notes = new List<Note>()
            };
        }

        [Test]
        public void GetHashCode_SameValues_ReturnsSameHashCode()
        {
            int hash1 = userFake.GetHashCode();
            int hash2 = sameUser.GetHashCode();

            Assert.That(hash1, Is.EqualTo(hash2));
        }

        [Test]
        public void GetHashCode_DifferentValues_ReturnsDifferentHashCode()
        {
            int hash1 = userFake.GetHashCode();
            int hash2 = differentUser.GetHashCode();

            Assert.That(hash1, Is.Not.EqualTo(hash2));
        }

        [Test]
        public void GetHashCode_MultipleCalls_ReturnsSameHashCode()
        {
            int hash1 = userFake.GetHashCode();
            int hash2 = userFake.GetHashCode();
            int hash3 = userFake.GetHashCode();

            Assert.Multiple(() =>
            {
                Assert.That(hash1, Is.EqualTo(hash2));
                Assert.That(hash1, Is.EqualTo(hash3));
            });
        }

        [Test]
        public void Equals_ParameterIsNull_ReturnsFalse()
        {
            bool result = userFake.Equals(null);
            Assert.That(result, Is.False);
        }

        [Test]
        public void Equals_ParameterIsNotUser_ReturnsFalse()
        {
            bool result = userFake.Equals(new Note());
            Assert.That(result, Is.False);
        }

        [Test]
        public void Equals_SameValues_ReturnsTrue()
        {
            bool result = userFake.Equals(sameUser);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Equals_DifferentValues_ReturnsFalse()
        {
            bool result = userFake.Equals(differentUser);
            Assert.That(result, Is.False);
        }
    }
}
