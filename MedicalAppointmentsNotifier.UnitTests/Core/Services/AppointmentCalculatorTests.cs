using MedicalAppointmentsNotifier.Core.Services;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace MedicalAppointmentsNotifier.UnitTests.Core.Services
{
    [TestFixture]
    public class AppointmentCalculatorTests
    {
        private Mock<IRepository<Appointment>> repositoryMock;
        private Mock<ILogger<AppointmentCalculator>> loggerMock;
        private AppointmentCalculator appointmentCalculator;

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IRepository<Appointment>>();
            loggerMock = new Mock<ILogger<AppointmentCalculator>>();
            appointmentCalculator = new AppointmentCalculator(repositoryMock.Object, loggerMock.Object);
        }

        [Test]
        public void Constructor_WhenCalledWithNullRepository_ThrowsArgumentNullException()
        {
            //Assert
            Assert.Throws<ArgumentNullException>(() => new AppointmentCalculator(null, loggerMock.Object));
        }

        [Test]
        public void Constructor_WhenCalledWithNullLogger_ThrowsArgumentNullException()
        {
            //Assert
            Assert.Throws<ArgumentNullException>(() => new AppointmentCalculator(repositoryMock.Object, null));
        }

        [Test]
        public void CalculateRemainingDays_WhenNextDateIsNull_ReturnsExpected()
        {
            //Arrange
            int expectedResult = 0;

            //Act
            int result = appointmentCalculator.CalculateRemainingDays(null);

            //Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void CalculateRemainingDays_WhenNextDateIsGiven_ReturnsCorrectResult()
        {
            //Arrange
            DateTimeOffset dateTimeFake = DateTimeOffset.Now.AddDays(5);
            int expectedResult = 4;

            //Act
            int result = appointmentCalculator.CalculateRemainingDays(dateTimeFake);

            //Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}
