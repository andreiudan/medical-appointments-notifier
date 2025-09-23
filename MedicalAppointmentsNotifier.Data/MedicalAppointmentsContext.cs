using MedicalAppointmentsNotifier.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointmentsNotifier.Data
{
    internal class MedicalAppointmentsContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<Note> Notes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source= ..\\MedicalAppointmentsNotifier.Data\\Database\\MedicalAppointments.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Appointments)
                .WithOne(a => a.User)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Notes)
                .WithOne(n => n.User)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
