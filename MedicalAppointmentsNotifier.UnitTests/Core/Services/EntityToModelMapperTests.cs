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
        private Mock<IAppointmentCalculator> appointmentCalculatorMock;
        private EntityToModelMapper mapper;
        private User userFake;

        [SetUp]
        public void SetUp()
        {
            appointmentCalculatorMock = new Mock<IAppointmentCalculator>();
            appointmentCalculatorMock.Setup(x => x.CalculateDaysUntilNextAppointmentAsync(It.IsAny<Guid>())).ReturnsAsync(0);

            mapper = new EntityToModelMapper(appointmentCalculatorMock.Object);

            userFake = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Appointments = new List<Appointment>(),
                Notes = new List<Note>()
            };
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
                User = userFake
            };

            AppointmentModel expectedResult = new AppointmentModel
            {
                Id = appointmentFake.Id,
                MedicalSpecialty = appointmentFake.MedicalSpecialty,
                MonthsInterval = appointmentFake.MonthsInterval,
                Status = appointmentFake.Status,
                IssuedOn = appointmentFake.IssuedOn,
                IsSelected = false
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
                Description = "Description",
                From = DateTimeOffset.Now,
            };

            NoteModel expectedResult = new NoteModel
            {
                Id = noteFake.Id,
                Description = noteFake.Description,
                From = noteFake.From,
                IsSelected = false
            };

            //Act
            var result = mapper.Map(noteFake);

            //Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void Map_WhenUserIsNull_ThrowsArgumentNullException()
        {
            //Asert
            User model = null;

            //Assert
            Assert.Throws<ArgumentNullException>(() => mapper.Map(model));
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
                DaysUntilNextAppointment = 0,
                Status = string.Empty,
                IsSelected = false
            };

            //Act
            var result = mapper.Map(userFake);

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
                IsSelected = false
            }; 

            Appointment expectedResult = new Appointment
            {
                Id = appointmentModelFake.Id,
                MedicalSpecialty = appointmentModelFake.MedicalSpecialty,
                MonthsInterval = appointmentModelFake.MonthsInterval,
                Status = appointmentModelFake.Status,
                IssuedOn = appointmentModelFake.IssuedOn,
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
                Description = "Description",
                From = DateTime.Now,
                IsSelected = false
            };

            Note expectedResult = new Note
            {
                Id = noteModelFake.Id,
                Description = noteModelFake.Description,
                From = noteModelFake.From,
                DaysPeriod = noteModelFake.DaysPeriod,
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
                DaysUntilNextAppointment = 0,
                Status = string.Empty,
                IsSelected = false
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
