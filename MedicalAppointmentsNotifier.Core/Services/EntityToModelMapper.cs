using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.Domain.Models;

namespace MedicalAppointmentsNotifier.Core.Services
{
    public class EntityToModelMapper : IEntityToModelMapper
    {
        private readonly IAppointmentCalculator appointmentCalculator;

        public EntityToModelMapper(IAppointmentCalculator appointmentCalculator)
        {
            this.appointmentCalculator = appointmentCalculator ?? throw new ArgumentNullException(nameof(appointmentCalculator));
        }

        public AppointmentModel Map(Appointment appointment)
        {
            ArgumentNullException.ThrowIfNull(appointment);

            AppointmentModel appointmentModel = new AppointmentModel
            {
                Id = appointment.Id,
                MedicalSpecialty = appointment.MedicalSpecialty,
                IntervalDays = appointment.IntervalDays,
                Status = appointment.Status,
                DaysUntilNextAppointment = appointmentCalculator.CalculateRemainingDays(appointment.NextDate),
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
                UserId = user.Id,
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
                UserId = user.Id
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
                DaysUntilNextAppointment = appointmentCalculator.CalculateDaysUntilNextAppointmentAsync(user.Id).Result,
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
    }
}
