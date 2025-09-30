using CommunityToolkit.Mvvm.DependencyInjection;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.Domain.Models;

namespace MedicalAppointmentsNotifier.Core.Services
{
    public class EntityToModelMapper : IEntityToModelMapper
    {
        public AppointmentModel Map(Appointment appointment)
        {
            ArgumentNullException.ThrowIfNull(appointment);

            AppointmentModel appointmentModel = new AppointmentModel
            {
                Id = appointment.Id,
                MedicalSpecialty = appointment.MedicalSpecialty,
                IntervalDays = appointment.IntervalDays,
                Status = appointment.Status,
                DaysUntilNextAppointment = CalculateRemainingDays(appointment.NextDate),
                LatestDate = appointment.LatestDate,
                NextDate = appointment.NextDate,
                IsSelected = false
            };

            return appointmentModel;
        }

        public NoteModel Map(Note note)
        {
            ArgumentNullException.ThrowIfNull(note);

            NoteModel noteModel = new NoteModel
            {
                Id = note.Id,
                Description = note.Description,
                From = note.From,
                Until = note.Until,
            };

            return noteModel;
        }

        public UserModel Map(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            UserModel userModel = new UserModel
            {
                Id = user.Id,
                Name = user.Name,
                DaysUntilNextAppointment = CalculateDaysUntilNextAppointment(user.Id).Result,
                Status = string.Empty,
                IsSelected = false
            };

            return userModel;
        }

        private int CalculateRemainingDays(DateTimeOffset? nextDate)
        {
            if (!nextDate.HasValue)
            {
                return 0;
            }

            int remainingDays = (nextDate.Value - DateTimeOffset.Now).Days;

            return remainingDays < 0 ? 0 : remainingDays;
        }

        private async Task<int> CalculateDaysUntilNextAppointment(Guid userId)
        {
            IRepository<Appointment> appointmentRepository = Ioc.Default.GetRequiredService<IRepository<Appointment>>();

            IEnumerable<Appointment> appointments = await appointmentRepository.FindAllAsync(a => a.User.Id == userId);

            DateTimeOffset? nextAppointment = appointments.OrderByDescending(a => a.NextDate).FirstOrDefault()?.NextDate;

            int daysUntilNextAppointment = nextAppointment.HasValue
                ? (nextAppointment.Value - DateTimeOffset.Now).Days
                : 0;

            return daysUntilNextAppointment;
        }
    }
}
