using MedicalAppointmentsNotifier.Domain.Entities;

namespace MedicalAppointmentsNotifier.Domain.Messages
{
    public class UserAddedMessage
    {
        public readonly User user;

        public UserAddedMessage(User user)
        {
            this.user = user ?? throw new ArgumentNullException(nameof(user));
        }
    }
}
