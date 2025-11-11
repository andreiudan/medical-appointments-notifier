using CommunityToolkit.Mvvm.DependencyInjection;
using MedicalAppointmentsNotifier.Data;
using MedicalAppointmentsNotifier.Infrastructure.DependencyInjection;
using MedicalAppointmentsNotifier.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Configuration;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

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
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            // Use a scope for DB migration so DbContext is not captured by the root provider
            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MedicalAppointmentsContext>();
                dbContext.Database.MigrateAsync();
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

            services.AddMedicalAppointmentsServices(connectionString);

            services.AddViewModels();

            return services.BuildServiceProvider();
        }
    }
}
