using MedicalAppointmentsNotifier.Domain.Messages;
using MedicalAppointmentsNotifier.Domain.Models;

namespace MedicalAppointmentsNotifier.UnitTests.Domain.Messages
{
    [TestFixture]
    public class UserMessagesTests
    {
        private UserModel userFake;

        [SetUp]
        public void SetUp()
        {
            userFake = new UserModel()
            {
                Id = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "Test"
            };
        }

        [Test]
        public void AddedConstructor_WhenUserIsNull_ThrowsArgumentNullException()
        {
            //Assert
            Assert.Throws<ArgumentNullException>(() => new UserAddedMessage(null));
        }

        [Test]
        public void AddedConstructor_Always_SetsUserField()
        {
            //Act
            UserAddedMessage message = new UserAddedMessage(userFake);

            //Assert
            Assert.That(message.user, Is.EqualTo(userFake));
        }

        [Test]
        public void UpdatedConstructor_WhenUserIsNull_ThrowsArgumentNullException()
        {
            //Assert
            Assert.Throws<ArgumentNullException>(() => new UserUpdatedMessage(null));
        }

        [Test]
        public void UpdatedConstructor_Always_SetsUserField()
        {
            //Act
            UserUpdatedMessage message = new UserUpdatedMessage(userFake);

            //Assert
            Assert.That(message.user, Is.EqualTo(userFake));
        }
    }
}
