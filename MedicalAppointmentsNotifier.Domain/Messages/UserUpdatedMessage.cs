using MedicalAppointmentsNotifier.Domain.Models;

namespace MedicalAppointmentsNotifier.Domain.Messages
{
    public class UserUpdatedMessage
    {
        public readonly UserModel user;

        public UserUpdatedMessage(UserModel user)
        {
            this.user = user ?? throw new ArgumentNullException(nameof(user));
        }
    }
}
