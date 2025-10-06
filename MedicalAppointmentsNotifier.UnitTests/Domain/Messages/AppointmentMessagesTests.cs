using MedicalAppointmentsNotifier.Domain.Messages;
using MedicalAppointmentsNotifier.Domain.Models;

namespace MedicalAppointmentsNotifier.UnitTests.Domain.Messages
{
    [TestFixture]
    public class AppointmentMessagesTests
    {
        private AppointmentModel appointmentFake;

        [SetUp]
        public void SetUp()
        {
            appointmentFake = new AppointmentModel()
            {
                Id = Guid.NewGuid(),
                MedicalSpecialty = 0,
                Status = 0,
                DaysUntilNextAppointment = 30,
                IntervalDays = 30,
                LatestDate = DateTime.Now,
                NextDate = DateTime.Now.AddDays(30),
                IsSelected = true,
            };
        }

        [Test]
        public void AddedConstructor_WhenAppointmentIsNull_ThrowsArgumentNullException()
        {
            //Assert
            Assert.Throws<ArgumentNullException>(() => new AppointmentAddedMessage(null));
        }

        [Test]
        public void AddedConstructor_Always_SetsAppointmentField()
        {
            //Act
            AppointmentAddedMessage message = new AppointmentAddedMessage(appointmentFake);

            //Assert
            Assert.That(message.appointment, Is.EqualTo(appointmentFake));
        }

        [Test]
        public void UpdatedConstructor_WhenAppointmentIsNull_ThrowsArgumentNullException()
        {
            //Assert
            Assert.Throws<ArgumentNullException>(() => new AppointmentUpdatedMessage(null));
        }

        [Test]
        public void UpdatedConstructor_Always_SetsAppointmentField()
        {
            //Act
            AppointmentUpdatedMessage message = new AppointmentUpdatedMessage(appointmentFake);

            //Assert
            Assert.That(message.appointment, Is.EqualTo(appointmentFake));
        }
    }
}
