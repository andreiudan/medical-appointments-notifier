using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MedicalAppointmentsNotifier.Data
{
    public class MedicalAppointmentsContextFactory : IDesignTimeDbContextFactory<MedicalAppointmentsContext>
    {
        public MedicalAppointmentsContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MedicalAppointmentsContext>();

            // Use a folder relative to the factory assembly location
            var projectFolder = Path.GetDirectoryName(typeof(MedicalAppointmentsContextFactory).Assembly.Location);
            string dbFolder = Path.Combine(projectFolder, "Database");

            if (!Directory.Exists(dbFolder))
                Directory.CreateDirectory(dbFolder);

            string databasePath = Path.Combine(dbFolder, "MedicalAppointments.db");
            optionsBuilder.UseSqlite($"Data Source={databasePath}");

            return new MedicalAppointmentsContext(optionsBuilder.Options);
        }
    }
}
