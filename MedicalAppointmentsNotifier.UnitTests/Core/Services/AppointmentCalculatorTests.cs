using MedicalAppointmentsNotifier.Core.Services;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

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

        [Test]
        public async Task CalculateDaysUntilNextAppointmentAsync_WhenUserIdIsEmpty_ReturnsZero()
        {
            //Arrange
            int expectedResult = 0;

            //Act
            int result = await appointmentCalculator.CalculateDaysUntilNextAppointmentAsync(Guid.Empty);

            //Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public async Task CalculateDaysUntilNextAppointmentAsync_WhenUserId_ReturnsCorrectResut()
        {
            //Arrange
            User userFake = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "Test",
            };

            IEnumerable<Appointment> appointmentsFake = new List<Appointment>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    MedicalSpecialty = 0,
                    MonthsInterval = 30,
                    Status = 0,
                    IssuedOn = DateTimeOffset.Now,
                    User = userFake
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    MedicalSpecialty = 0,
                    MonthsInterval = 30,
                    Status = 0,
                    IssuedOn = DateTimeOffset.Now,
                    User = userFake
                }
            };

            repositoryMock.Setup(r => r.FindAllAsync(It.IsAny<Expression<Func<Appointment, bool>>>())).ReturnsAsync(appointmentsFake);

            int expectedResult = 4;

            //Act
            int result = await appointmentCalculator.CalculateDaysUntilNextAppointmentAsync(userFake.Id);

            //Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}
