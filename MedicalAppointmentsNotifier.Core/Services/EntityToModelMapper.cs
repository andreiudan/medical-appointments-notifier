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
                MonthsInterval = appointment.MonthsInterval,
                Status = appointment.Status,
                IssuedOn = appointment.IssuedOn,
                ScheduledOn = appointment.ScheduledOn,
                ScheduledLocation = appointment.ScheduledLocation,
                IsSelected = false
            };

            return appointmentModel;
        }

        public Appointment Map(AppointmentModel appointmentModel, Guid userId)
        {
            ArgumentNullException.ThrowIfNull(appointmentModel);

            Appointment appointment = new Appointment
            {
                Id = appointmentModel.Id,
                MedicalSpecialty = appointmentModel.MedicalSpecialty,
                MonthsInterval = appointmentModel.MonthsInterval,
                Status = appointmentModel.Status,
                IssuedOn = appointmentModel.IssuedOn,
                ScheduledOn = appointmentModel.ScheduledOn,
                ScheduledLocation = appointmentModel.ScheduledLocation,
                UserId = userId,
            };

            return appointment;
        }

        public NoteModel Map(Note note)
        {
            ArgumentNullException.ThrowIfNull(note);

            NoteModel noteModel = new NoteModel
            {
                Id = note.Id,
                Title = note.Title,
                Description = note.Description,
                From = note.From,
                MonthsPeriod = note.MonthsPeriod,
            };

            return noteModel;
        }

        public Note Map(NoteModel noteModel, Guid userId)
        {
            ArgumentNullException.ThrowIfNull(noteModel);

            Note note = new Note
            {
                Id = noteModel.Id,
                Title = noteModel.Title,
                Description = noteModel.Description,
                From = noteModel.From,
                MonthsPeriod = noteModel.MonthsPeriod,
                UserId = userId
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
                DaysUntilNextAppointment = 0,
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
