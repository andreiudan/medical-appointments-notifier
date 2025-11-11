using MedicalAppointmentsNotifier.Core.Services;
using MedicalAppointmentsNotifier.Core.ViewModels;
using MedicalAppointmentsNotifier.Data;
using MedicalAppointmentsNotifier.Data.Repositories;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MedicalAppointmentsNotifier.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMedicalAppointmentsServices(this IServiceCollection services, string connectionString)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            connectionString = connectionString.Replace("{AppData}", appDataPath);

            string dbPath = connectionString.Replace("Data Source=", "");
            string dbFolder = Path.GetDirectoryName(dbPath);
            if (!Directory.Exists(dbFolder))
            {
                Directory.CreateDirectory(dbFolder);
            }

            services.AddDbContext<MedicalAppointmentsContext>(options =>
                options.UseSqlite(connectionString));

            services.AddTransient<IRepository<User>, Repository<User>>();
            services.AddTransient<IRepository<Appointment>, Repository<Appointment>>();
            services.AddTransient<IAppointmentsRepository, AppointmentsRepository>();
            services.AddTransient<IRepository<Note>, Repository<Note>>();

            services.AddScoped<IAppointmentCalculator, AppointmentCalculator>();
            services.AddScoped<IAppointmentScanner, AppointmentScanner>();
            services.AddScoped<IEntityToModelMapper, EntityToModelMapper>();

            return services;
        }

        public static IServiceCollection AddViewModels(this IServiceCollection services)
        {
            services.AddScoped<UsersViewModel>();
            services.AddTransient<UserAppointmentsViewModel>();
            services.AddTransient<UpsertUserViewModel>();
            services.AddTransient<UpsertNoteViewModel>();
            services.AddTransient<UpsertAppointmentViewModel>();

            return services;
        }
    }
}