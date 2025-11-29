using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.EntityPropertyTypes;

namespace MedicalAppointmentsNotifier.UnitTests.Domain.Entities
{
    [TestFixture]
    public class AppointmentTests
    {
        private Appointment appointmentFake;
        private Appointment differentAppointment;
        private Appointment sameAppointment;

        [SetUp]
        public void SetUp()
        {
            var user = new User { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };

            appointmentFake = new Appointment
            {
                Id = Guid.NewGuid(),
                MedicalSpecialty = 0,
                MonthsInterval = 30,
                Status = 0,
                IssuedOn = DateTimeOffset.Now,
                User = user
            };

            differentAppointment = new Appointment
            {
                Id = Guid.NewGuid(),
                MedicalSpecialty = 0,
                MonthsInterval = 60,
                Status = 0,
                IssuedOn = DateTimeOffset.Now.AddDays(-30),
                User = new User { Id = Guid.NewGuid(), FirstName = "Alex", LastName = "Bean" }
            };

            sameAppointment = new Appointment
            {
                Id = appointmentFake.Id,
                MedicalSpecialty = 0,
                MonthsInterval = 30,
                Status = 0,
                IssuedOn = appointmentFake.IssuedOn,
                User = user
            };
        }

        [Test]
        public void GetHashCode_SameValues_ReturnsSameHashCode()
        {
            int hash1 = appointmentFake.GetHashCode();
            int hash2 = sameAppointment.GetHashCode();

            Assert.That(hash1, Is.EqualTo(hash2));
        }

        [Test]
        public void GetHashCode_DifferentValues_ReturnsDifferentHashCode()
        {
            int hash1 = appointmentFake.GetHashCode();
            int hash2 = differentAppointment.GetHashCode();

            Assert.That(hash1, Is.Not.EqualTo(hash2));
        }

        [Test]
        public void GetHashCode_MultipleCalls_ReturnsSameHashCode()
        {
            int hash1 = appointmentFake.GetHashCode();
            int hash2 = appointmentFake.GetHashCode();
            int hash3 = appointmentFake.GetHashCode();

            Assert.Multiple(() =>
            {
                Assert.That(hash1, Is.EqualTo(hash2));
                Assert.That(hash1, Is.EqualTo(hash3));
            });
        }

        [Test]
        public void Equals_ParameterIsNull_ReturnsFalse()
        {
            bool result = appointmentFake.Equals(null);
            Assert.That(result, Is.False);
        }

        [Test]
        public void Equals_ParameterIsNotAppointment_ReturnsFalse()
        {
            bool result = appointmentFake.Equals(new Note());
            Assert.That(result, Is.False);
        }

        [Test]
        public void Equals_SameValues_ReturnsTrue()
        {
            bool result = appointmentFake.Equals(sameAppointment);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Equals_DifferentValues_ReturnsFalse()
        {
            bool result = appointmentFake.Equals(differentAppointment);
            Assert.That(result, Is.False);
        }
    }
}
