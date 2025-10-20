using MedicalAppointmentsNotifier.Data;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MedicalAppointmentsNotifier.ReminderJob
{
    internal class Program
    {
        //[DllImport("kernel32.dll")]
        //static extern IntPtr GetConsoleWindow();

        //[DllImport("user32.dll")]
        //static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        //const int SW_HIDE = 0;

        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    string connectionString = context.Configuration.GetConnectionString("DefaultConnection");

                    services.AddMedicalAppointmentsServices(connectionString);

                    services.AddScoped<NotifierWorker>();

                    services.AddLogging(builder =>
                    {
                        builder.AddConsole();
                        builder.SetMinimumLevel(LogLevel.Information);
                    });
                })
                .Build();

            try
            {
                Console.WriteLine("Medical Appointments Notifier - Scheduled Task");
                Console.WriteLine($"Started at: {DateTime.Now}");
                Console.WriteLine();

                var dbContext = host.Services.GetRequiredService<MedicalAppointmentsContext>();
                var appointmentsRepo = host.Services.GetRequiredService<IAppointmentsRepository>();
                var logger = host.Services.GetRequiredService<ILogger<Program>>();

                await dbContext.Database.MigrateAsync();
                logger.LogInformation("Database migration completed");

                NotifierWorker notifierWorker = host.Services.GetRequiredService<NotifierWorker>();
                await notifierWorker.RunAsync();

                Console.WriteLine();
                Console.WriteLine("Task completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Environment.Exit(1);
            }

            //nint handle = GetConsoleWindow();
            //ShowWindow(handle, SW_HIDE);
        }
    }
}