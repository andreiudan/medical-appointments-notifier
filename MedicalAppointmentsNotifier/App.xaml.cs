using CommunityToolkit.Mvvm.DependencyInjection;
using MedicalAppointmentsNotifier.Data;
using MedicalAppointmentsNotifier.Infrastructure.DependencyInjection;
using MedicalAppointmentsNotifier.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Configuration;
using WinRT.Interop;

namespace MedicalAppointmentsNotifier
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;
        public Frame RootFrame { get; private set; }
        public IServiceProvider Services { get; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Services = ConfigureServices();
            Ioc.Default.ConfigureServices(Services);
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MedicalAppointmentsContext>();
                await dbContext.Database.MigrateAsync();
            }

            _window = new MainWindow();

            RootFrame = new Frame();
            RootFrame.Navigate(typeof(UsersView), args.Arguments);

            _window.Content = RootFrame;
            _window.Activate();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

            services.AddLoggingService();
            services.AddMedicalAppointmentsServices(connectionString);
            services.AddViewModels();

            return services.BuildServiceProvider();
        }
    }
}
