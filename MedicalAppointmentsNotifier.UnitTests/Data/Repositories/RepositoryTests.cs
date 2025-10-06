using MedicalAppointmentsNotifier.Data;
using MedicalAppointmentsNotifier.Data.Repositories;
using MedicalAppointmentsNotifier.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MedicalAppointmentsNotifier.UnitTests.Data.Repositories
{
    [TestFixture]
    public class RepositoryTests
    {
        private Mock<MedicalAppointmentsContext> contextMock;
        private Repository<User> repositoryFake;
        private IList<User> usersFake;

        [SetUp]
        public void SetUp()
        {
            usersFake = new List<User>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    FirstName = "FTest1",
                    LastName = "LTest2",
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    FirstName = "FTest2",
                    LastName = "LTest2",
                }
            };

            var options = new DbContextOptionsBuilder<MedicalAppointmentsContext>()
                    .Options;

            contextMock = new Mock<MedicalAppointmentsContext>(options);

            contextMock.Setup(x => x.Set<User>()).ReturnsDbSet(usersFake);
            contextMock.Setup(x => x.Set<User>().FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((object[] ids) =>
                {
                    var id = (Guid)ids[0];
                    return usersFake.FirstOrDefault(u => u.Id == id);
                });

            repositoryFake = new Repository<User>(contextMock.Object);
        }

        [Test]
        public void Constructor_WhenContextIsNull_ThrowsArgumentNullException()
        {
            //Assert
            Assert.Throws<ArgumentNullException>(() => new Repository<User>(null));
        }

        [Test]
        public async Task GetAllAsync_Always_ReturnsAllEntitesOfGivenType()
        {
            //Act
            List<User> result = await repositoryFake.GetAllAsync();

            //Assert
            Assert.That(result, Is.EqualTo(usersFake));
        }

        [Test]
        public async Task DeleteAsync_AlwaysCalledWithUser_DeletesTheUser()
        {
            //Act
            bool result = await repositoryFake.DeleteAsync(usersFake[1]);

            //Assert
            Assert.That(result, Is.True);
            contextMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Test]
        public void DeleteAsync_WhenCalledWithNullUser_ThrowsArgumentNullException()
        {
            //Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await repositoryFake.DeleteAsync(null));
        }

        [Test]
        public void DeleteAsync_WhenCalledWithNullUserId_ThrowsArgumentNullException()
        {
            //Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await repositoryFake.DeleteAsync(null));
        }

        [Test]
        public async Task DeleteAsync_WhenCalledWithUnknownUserId_ReturnsExpectedResult()
        {
            //Act
            bool result = await repositoryFake.DeleteAsync(Guid.NewGuid());

            //Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DeleteAsync_AlwaysCalledWithUserId_DeletesTheUser()
        {
            //Act
            bool result = await repositoryFake.DeleteAsync(usersFake[0].Id);

            //Assert
            Assert.That(result, Is.True);
            contextMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Test]
        public void FindAllAsync_WhenParameterIsNull_ThrowsArgumentNullException()
        {
            //Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await repositoryFake.FindAllAsync(It.IsAny<Expression<Func<User, bool>>>()));
        }

        [Test]
        public async Task FindAllAsync_Always_ReturnAllMatchingEntries()
        {
            //Arrange
            string lastName = usersFake[0].LastName;

            //Act
            IEnumerable<User> result = await repositoryFake.FindAllAsync(n => n.LastName.Equals(lastName));

            //Assert
            Assert.That(result, Is.EquivalentTo(usersFake));
        }

        [Test]
        public void FindAsync_WhenParameterIsNull_ThrowsArgumentNullException()
        {
            //Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await repositoryFake.FindAsync(It.IsAny<Expression<Func<User, bool>>>()));
        }

        [Test]
        public async Task FindAsync_Always_ReturnMatchingEntry()
        {
            //Arrange
            string firstName = usersFake[0].FirstName;

            //Act
            User result = await repositoryFake.FindAsync(n => n.FirstName.Equals(firstName));

            //Assert
            Assert.That(result, Is.EqualTo(usersFake[0]));
        }

        [Test]
        public void UpdateAsync_WhenParameterIsNull_ThrowsArgumentNullException()
        {
            //Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await repositoryFake.UpdateAsync(null));
        }

        [Test]
        public async Task UpdateAsync_WhenGivenEntryIsIncorrect_ReturnsFalse()
        {
            //Arrange
            User userFake = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "New",
                LastName = "User"
            };

            //Act
            bool result = await repositoryFake.UpdateAsync(userFake);

            //Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void AddAsync_WhenParameterIsNull_ThrowsArgumentNullException()
        {
            //Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await repositoryFake.AddAsync(null));
        }

        [Test]
        public async Task AddAsync_Always_AddsEntry()
        {
            //Arrange
            User userFake = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "New",
                LastName = "User"
            };

            //Act
            User result = await repositoryFake.AddAsync(userFake);

            //Assert
            Assert.That(result, Is.EqualTo(userFake));
            contextMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }
    }
}
