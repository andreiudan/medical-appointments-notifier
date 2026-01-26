using MedicalAppointmentsNotifier.Core.Services;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.Domain.Models;
using Moq;

namespace MedicalAppointmentsNotifier.UnitTests.Core.Services
{
    [TestFixture]
    public class EntityToModelMapperTests
    {
        private Mock<IAppointmentsRepository> appointmentRepositoryMock;
        private EntityToModelMapper mapper;
        private User userFake;

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

            appointmentRepositoryMock = new Mock<IAppointmentsRepository>();
            appointmentRepositoryMock.Setup(x => x.GetExpiringAppointmentsCount(userFake.Id)).ReturnsAsync(4);
            appointmentRepositoryMock.Setup(x => x.GetUpcomingAppointmentsCount(userFake.Id)).ReturnsAsync(0);

            mapper = new EntityToModelMapper(appointmentRepositoryMock.Object);
        }

        [Test]
        public void Constructor_WhenCalledWithNullRepository_ThrowsArgumentNullException()
        {
            //Assert
            Assert.Throws<ArgumentNullException>(() => new EntityToModelMapper(null));
        }

        [Test]
        public void Map_WhenAppointmentIsNull_ThrowsArgumentNullException()
        {
            //Asert
             Appointment model = null;

            //Assert
            Assert.Throws<ArgumentNullException>(() => mapper.Map(model));
        }

        [Test]
        public void Map_WhenAppointmentIsNotNull_CorrectlyMapsToAppointmentModel()
        {
            //Asert
            Appointment appointmentFake = new Appointment
            {
                Id = Guid.NewGuid(),
                MedicalSpecialty = 0,
                MonthsInterval = 30,
                Status = 0,
                IssuedOn = DateTimeOffset.Now,
                User = userFake,
                ScheduledOn = DateTimeOffset.Now.AddDays(10),
                ScheduledLocation = "Location"
            };

            AppointmentModel expectedResult = new AppointmentModel
            {
                Id = appointmentFake.Id,
                MedicalSpecialty = appointmentFake.MedicalSpecialty,
                MonthsInterval = appointmentFake.MonthsInterval,
                Status = appointmentFake.Status,
                IssuedOn = appointmentFake.IssuedOn,
                ScheduledOn = appointmentFake.ScheduledOn,
                ScheduledLocation = appointmentFake.ScheduledLocation,
            };

            //Act
            var result = mapper.Map(appointmentFake);

            //Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void Map_WhenNoteIsNull_ThrowsArgumentNullException()
        {
            //Asert
            Note model = null;

            //Assert
            Assert.Throws<ArgumentNullException>(() => mapper.Map(model));
        }

        [Test]
        public void Map_WhenNoteIsNotNull_CorrectlyMapsToNoteModel()
        {
            //Asert
            Note noteFake = new Note
            {
                Id = Guid.NewGuid(),
                Title = "Title",
                Description = "Description",
                From = DateTimeOffset.Now,
                MonthsPeriod = 6,
            };

            NoteModel expectedResult = new NoteModel
            {
                Id = noteFake.Id,
                Title = noteFake.Title,
                Description = noteFake.Description,
                From = noteFake.From,
                MonthsPeriod = noteFake.MonthsPeriod
            };

            //Act
            var result = mapper.Map(noteFake);

            //Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public async Task Map_WhenUserIsNull_ThrowsArgumentNullException()
        {
            //Asert
            User model = null;

            //Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => mapper.Map(model));
        }

        [Test]
        public async Task Map_WhenUserIsNotNull_CorrectlyMapsToUserModel()
        {
            //Asert
            UserModel expectedResult = new UserModel
            {
                Id = userFake.Id,
                FirstName = userFake.FirstName,
                LastName = userFake.LastName,
                ExpiringAppointmentsCount = 4,
                UpcominAppointmentsCount = 0
            };

            //Act
            var result = await mapper.Map(userFake);

            //Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void Map_WhenAppointmentModelIsNull_ThrowsArgumentNullException()
        {
            //Asert
            AppointmentModel model = null;

            //Assert
            Assert.Throws<ArgumentNullException>(() => mapper.Map(model, userFake.Id));
        }

        [Test]
        public void Map_WhenAppointmentModelIsNotNull_CorrectlyMapsToAppointment()
        {
            //Asert
            AppointmentModel appointmentModelFake = new AppointmentModel
            {
                Id = Guid.NewGuid(),
                MedicalSpecialty = 0,
                MonthsInterval = 30,
                Status = 0,
                IssuedOn = DateTime.Now,
                ScheduledOn = DateTime.Now.AddDays(10),
                ScheduledLocation = "Location"
            }; 

            Appointment expectedResult = new Appointment
            {
                Id = appointmentModelFake.Id,
                MedicalSpecialty = appointmentModelFake.MedicalSpecialty,
                MonthsInterval = appointmentModelFake.MonthsInterval,
                Status = appointmentModelFake.Status,
                IssuedOn = appointmentModelFake.IssuedOn,
                ScheduledOn = appointmentModelFake.ScheduledOn,
                ScheduledLocation = appointmentModelFake.ScheduledLocation,
                UserId = userFake.Id
            };

            //Act
            var result = mapper.Map(appointmentModelFake, userFake.Id);

            //Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void Map_WhenNoteModelIsNull_ThrowsArgumentNullException()
        {
            //Asert
            NoteModel model = null;

            //Assert
            Assert.Throws<ArgumentNullException>(() => mapper.Map(model, userFake.Id));
        }

        [Test]
        public void Map_WhenNoteModelIsNotNull_CorrectlyMapsToNote()
        {
            //Asert
            NoteModel noteModelFake = new NoteModel
            {
                Id = Guid.NewGuid(),
                Title = "Title",
                Description = "Description",
                From = DateTime.Now,
                MonthsPeriod = 3
            };

            Note expectedResult = new Note
            {
                Id = noteModelFake.Id,
                Title = noteModelFake.Title,
                Description = noteModelFake.Description,
                From = noteModelFake.From,
                MonthsPeriod = noteModelFake.MonthsPeriod,
                UserId = userFake.Id
            };

            //Act
            var result = mapper.Map(noteModelFake, userFake.Id);

            //Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void Map_WhenUserModelIsNull_ThrowsArgumentNullException()
        {
            //Asert
            UserModel model = null;

            //Assert
            Assert.Throws<ArgumentNullException>(() => mapper.Map(model));
        }

        [Test]
        public void Map_WhenUserModelIsNotNull_CorrectlyMapsToUser()
        {
            //Asert
            UserModel userModelFake = new UserModel
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                ExpiringAppointmentsCount = 4,
                UpcominAppointmentsCount = 0
            };

            User expectedResult = new User
            {
                Id = userModelFake.Id,
                FirstName = userModelFake.FirstName, 
                LastName = userModelFake.LastName
            };

            //Act
            var result = mapper.Map(userModelFake);

            //Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}
