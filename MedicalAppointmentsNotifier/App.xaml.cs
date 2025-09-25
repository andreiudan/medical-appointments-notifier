using CommunityToolkit.Mvvm.DependencyInjection;
using MedicalAppointmentsNotifier.Core.ViewModels;
using MedicalAppointmentsNotifier.Data;
using MedicalAppointmentsNotifier.Data.Repositories;
using MedicalAppointmentsNotifier.Domain.Entities;
using MedicalAppointmentsNotifier.Domain.Interfaces;
using MedicalAppointmentsNotifier.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using System.IO;

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
            _window = new MainWindow();

            RootFrame = new Frame();
            RootFrame.Navigate(typeof(UsersView), args.Arguments);

            _window.Content = RootFrame;
            _window.Activate();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            string databasePath = GetDatabasePath();

            services.AddDbContext<MedicalAppointmentsContext>(options =>
            {
                options.UseSqlite($"Data Source={databasePath}");
            });

            services.AddScoped<IRepository<User>, Repository<User>>();
            services.AddTransient<UsersViewModel>();

            return services.BuildServiceProvider();
        }

        private static string GetDatabasePath()
        {
            var baseDir = AppContext.BaseDirectory;
            var repoRoot = Directory.GetParent(baseDir)
                ?.Parent
                ?.Parent
                ?.Parent
                ?.Parent
                ?.Parent
                ?.Parent
                ?.Parent
                ?.FullName ?? throw new InvalidOperationException("Cannot find repo root");

            string dbFolder = Path.Combine(repoRoot, @"MedicalAppointmentsNotifier.Data\Database");

            if (!Directory.Exists(dbFolder))
            {
                Directory.CreateDirectory(dbFolder);
            }

            return Path.Combine(dbFolder, "MedicalAppointments.db");
        }
    }
}
