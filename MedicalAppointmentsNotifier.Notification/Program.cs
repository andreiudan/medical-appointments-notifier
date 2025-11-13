using MedicalAppointmentsNotifier.Data;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Uwp.Notifications;

namespace MedicalAppointmentsNotifier.ReminderJob
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                Environment.Exit(0);
            };

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    string connectionString = context.Configuration.GetConnectionString("DefaultConnection");

                    services.AddMedicalAppointmentsServices(connectionString);

                    services.AddScoped<NotifierWorker>();

                    services.AddLoggingService();
                })
                .Build();

            await host.StartAsync();

            using var scope = host.Services.CreateScope();
            ILogger<Program> logger = host.Services.GetRequiredService<ILogger<Program>>();

            try
            {
                IAppointmentsRepository appointmentsRepo = host.Services.GetRequiredService<IAppointmentsRepository>();

                logger.LogInformation("Medical Appointments Notifier - Scheduled Task started at {StartTime}", DateTime.Now);

                using (var dbMigrationScope = host.Services.CreateScope())
                {
                    MedicalAppointmentsContext scopedDbContext = dbMigrationScope.ServiceProvider.GetRequiredService<MedicalAppointmentsContext>();
                    await scopedDbContext.Database.MigrateAsync();
                }

                logger.LogInformation("Starting Notifier Worker...");
                NotifierWorker notifierWorker = host.Services.GetRequiredService<NotifierWorker>();
                await notifierWorker.RunAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while running the Notifier Worker.");
                Environment.Exit(1);
            }

            logger.LogInformation("Notifier Worker completed successfully.");
            await host.StopAsync();
            host.Dispose();
        }
    }
}