namespace MedicalAppointmentsNotifier.Domain.Messages
{
    public class RefreshUserMessage
    {
        public readonly Guid userId;

        public RefreshUserMessage(Guid userId)
        {
            if(userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }

            this.userId = userId;
        }
    }
}
