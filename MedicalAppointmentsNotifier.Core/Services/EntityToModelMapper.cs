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

        public Appointment Map(AppointmentModel appointmentModel, User user)
        {
            ArgumentNullException.ThrowIfNull(appointmentModel);

            Appointment appointment = new Appointment
            {
                Id = appointmentModel.Id,
                MedicalSpecialty = appointmentModel.MedicalSpecialty,
                IntervalDays = appointmentModel.IntervalDays,
                Status = appointmentModel.Status,
                LatestDate = appointmentModel.LatestDate,
                NextDate = appointmentModel.NextDate,
                User = user
            };

            return appointment;
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

        public Note Map(NoteModel noteModel, User user)
        {
            ArgumentNullException.ThrowIfNull(noteModel);

            Note note = new Note
            {
                Id = noteModel.Id,
                Description = noteModel.Description,
                From = noteModel.From,
                Until = noteModel.Until,
                User = user
            };

            return note;
        }

        public UserModel Map(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            UserModel userModel = new UserModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DaysUntilNextAppointment = CalculateDaysUntilNextAppointment(user.Id).Result,
                Status = string.Empty,
                IsSelected = false
            };

            return userModel;
        }

        public User Map(UserModel userModel)
        {
            ArgumentNullException.ThrowIfNull(userModel);

            User user = new User
            {
                Id = userModel.Id,
                FirstName= userModel.FirstName,
                LastName = userModel.LastName,
            };

            return user;
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
