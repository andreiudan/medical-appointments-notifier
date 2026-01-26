using MedicalAppointmentsNotifier.Domain.Models;

namespace MedicalAppointmentsNotifier.Domain.Messages
{
    public class UserAddedMessage
    {
        public readonly UserModel user;

        public UserAddedMessage(UserModel user)
        {
            this.user = user ?? throw new ArgumentNullException(nameof(user));
        }
    }
}
