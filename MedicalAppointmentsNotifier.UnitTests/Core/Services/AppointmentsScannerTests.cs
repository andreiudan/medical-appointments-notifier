using MedicalAppointmentsNotifier.Core.Services;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.EntityPropertyTypes;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace MedicalAppointmentsNotifier.UnitTests.Core.Services
{
    [TestFixture]
    public class AppointmentsScannerTests
    {
        private Mock<IAppointmentsRepository> appointemntsRepositoryMock;
        private Mock<ILogger<AppointmentScanner>> loggerMock;
        private AppointmentScanner scanner;

        [SetUp]
        public void Setup()
        {
            appointemntsRepositoryMock = new Mock<IAppointmentsRepository>();
            loggerMock = new Mock<ILogger<AppointmentScanner>>();
            scanner = new AppointmentScanner(appointemntsRepositoryMock.Object, loggerMock.Object);
        }

        [Test]
        public void Constructor_WhenCalledWithNullRepository_ThrowsArgumentNullException()
        {
            //Assert
            Assert.Throws<ArgumentNullException>(() => new AppointmentScanner(null, loggerMock.Object));
        }

        [Test]
        public void Constructor_WhenCalledWithNullLogger_ThrowsArgumentNullException()
        {
            //Assert
            Assert.Throws<ArgumentNullException>(() => new AppointmentScanner(appointemntsRepositoryMock.Object, null));
        }

        [Test]
        public async Task GetExpiredAppointmentsMessage_WhenThereAreNoExpiringAppointments_ReturnsEmptyString()
        {
            //Arrange
            appointemntsRepositoryMock.Setup(x => x.GetAllExpiringAppointments()).ReturnsAsync(new List<Appointment>());

            //Act
            string result = await scanner.GetExpiredAppointmentsMessage();

            //Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public async Task GetExpiredAppointmentsMessage_WhenThereAreExpiringAppointments_ReturnsMessageInCorrectFormat()
        {
            //Arrange
            List<Appointment> appointments = new List<Appointment>
            {
                new Appointment
                {
                    User = new User { FirstName = "Jane", LastName = "Smith" },
                    MedicalSpecialty = MedicalSpecialty.Cardiologie,
                    IssuedOn = DateTimeOffset.Now.AddDays(-20),
                    MonthsInterval = 1,
                    Status = AppointmentStatus.Neprogramat
                },
                new Appointment
                {
                    User = new User { FirstName = "John", LastName = "Doe" },
                    MedicalSpecialty = MedicalSpecialty.Oncologie,
                    IssuedOn = DateTimeOffset.Now.AddDays(-25),
                    MonthsInterval = 1,
                    Status = AppointmentStatus.Neprogramat,
                }
            };

            appointemntsRepositoryMock.Setup(x => x.GetAllExpiringAppointments()).ReturnsAsync(appointments);

            string expectedMessage =
                "0.Doe John - Oncologie - peste 5 zile.\r\n" +
                "1.Smith Jane - Cardiologie - peste 10 zile.\r\n";

            //Act
            string result = await scanner.GetExpiredAppointmentsMessage();

            //Assert
            Assert.That(result, Is.EqualTo(expectedMessage));
        }
    }
}
