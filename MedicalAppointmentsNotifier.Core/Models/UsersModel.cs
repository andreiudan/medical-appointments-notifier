using MedicalAppointmentsNotifier.Core.Services;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;

namespace MedicalAppointmentsNotifier.Core.Models
{
    internal class UsersModel
    {
        private readonly IRepository<User> usersRepository;
        private readonly NameCorrector nameCorrector;

        public UsersModel()
        {

        }

        public User Get(int id)
        {
            Guid userId;

            if (!Guid.TryParse(id.ToString(), out userId))
            {
                throw new ArgumentException("Invalid user ID format.");
            }

            var user = usersRepository.GetById(userId);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            return user;
        }

        public async Task<List<User>> GetAll()
        {
            return await usersRepository.GetAll();
        }

        public User Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("User name cannot be empty.");
            }

            nameCorrector.CorrectName(name);

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Name = name
            };

            return usersRepository.Add(newUser);
        }

        public bool Delete(int id)
        {
            Guid userId;

            if (!Guid.TryParse(id.ToString(), out userId))
            {
                throw new ArgumentException("Invalid user ID format.");
            }

            var user = usersRepository.GetById(userId);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            usersRepository.Delete(user);

            return true;
        }

        public User Update(int id, string name)
        {
            Guid userId;

            if (!Guid.TryParse(id.ToString(), out userId))
            {
                throw new ArgumentException("Invalid user ID format.");
            }

            var user = usersRepository.GetById(userId);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("User name cannot be empty.");
            }

            nameCorrector.CorrectName(name);

            user.Name = name;

            return usersRepository.Update(user);
        }
    }
}
