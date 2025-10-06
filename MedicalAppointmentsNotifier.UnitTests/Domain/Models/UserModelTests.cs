using MedicalAppointmentsNotifier.Domain.Models;

namespace MedicalAppointmentsNotifier.UnitTests.Domain.Models
{
    [TestFixture]
    public class UserModelTests
    {
        private UserModel userFake;
        private UserModel differentUser;
        private UserModel sameUser;

        [SetUp]
        public void SetUp()
        {
            userFake = new UserModel
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                DaysUntilNextAppointment = 5,
                Status = "Active",
                IsSelected = true
            };

            differentUser = new UserModel
            {
                Id = Guid.NewGuid(),
                FirstName = "Alex",
                LastName = "Bean",
                DaysUntilNextAppointment = 5,
                Status = "Inactive",
                IsSelected = false
            };

            sameUser = new UserModel
            {
                Id = userFake.Id,
                FirstName = "John",
                LastName = "Doe",
                DaysUntilNextAppointment = 5,
                Status = "Active",
                IsSelected = true
            };
        }

        [Test]
        public void GetHashCode_SameValues_ReturnsSameHashCode()
        {
            // Act
            int hash1 = userFake.GetHashCode();
            int hash2 = sameUser.GetHashCode();

            // Assert
            Assert.That(hash1, Is.EqualTo(hash2));
        }

        [Test]
        public void GetHashCode_DifferentValues_ReturnsSameHashCode()
        {
            // Act
            int hash1 = userFake.GetHashCode();
            int hash2 = differentUser.GetHashCode();

            // Assert
            Assert.That(hash1, Is.Not.EqualTo(hash2));
        }

        [Test]
        public void GetHashCode_MultipleCalls_ReturnsSameHashCode()
        {
            // Act
            int hash1 = userFake.GetHashCode();
            int hash2 = userFake.GetHashCode();
            int hash3 = userFake.GetHashCode();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(hash1, Is.EqualTo(hash2));
                Assert.That(hash1, Is.EqualTo(hash3));
            });
        }

        [Test]
        public void Equals_ParameterIsNull_ReturnsFalse()
        {
            //Act
            bool result = userFake.Equals(null);

            //Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void Equals_ParameterIsNotUserModel_ReturnsFalse()
        {
            //Act
            bool result = userFake.Equals(new NoteModel());

            //Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void Equals_SameValues_ReturnsTrue()
        {
            //Act
            bool result = userFake.Equals(sameUser);

            //Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void Equals_DifferentValues_ReturnsFalse()
        {
            //Act
            bool result = userFake.Equals(differentUser);

            //Assert
            Assert.That(result, Is.False);
        }
    }
}
