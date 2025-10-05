using MedicalAppointmentsNotifier.Domain.EntityPropertyTypes;
using MedicalAppointmentsNotifier.Domain.Models;

namespace MedicalAppointmentsNotifier.UnitTests.Domain.Models
{
    [TestFixture]
    public class AppointmentModelTests
    {
        private AppointmentModel appointmentFake;
        private AppointmentModel differentAppointment;
        private AppointmentModel sameAppointment;

        [SetUp]
        public void SetUp()
        {
            appointmentFake = new AppointmentModel
            {
                Id = Guid.NewGuid(),
                MedicalSpecialty = 0,
                IntervalDays = 30,
                Status = 0,
                DaysUntilNextAppointment = 10,
                LatestDate = DateTimeOffset.Now,
                NextDate = DateTimeOffset.Now.AddDays(10),
                IsSelected = true
            };

            differentAppointment = new AppointmentModel
            {
                Id = Guid.NewGuid(),
                MedicalSpecialty = 0,
                IntervalDays = 60,
                Status = 0,
                DaysUntilNextAppointment = 0,
                LatestDate = DateTimeOffset.Now.AddDays(-60),
                NextDate = DateTimeOffset.Now,
                IsSelected = false
            };

            sameAppointment = new AppointmentModel
            {
                Id = appointmentFake.Id,
                MedicalSpecialty = 0,
                IntervalDays = 30,
                Status = 0,
                DaysUntilNextAppointment = 10,
                LatestDate = appointmentFake.LatestDate,
                NextDate = appointmentFake.NextDate,
                IsSelected = true
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
        public void Equals_ParameterIsNotAppointmentModel_ReturnsFalse()
        {
            bool result = appointmentFake.Equals(new NoteModel());
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
