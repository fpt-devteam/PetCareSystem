using Microsoft.EntityFrameworkCore;
using VetClinic.Repository.Entities;
using VetClinic.Repository.Data.Configurations;

namespace VetClinic.Repository.Data
{
    public class VetClinicDbContext : DbContext
    {
        public VetClinicDbContext(DbContextOptions<VetClinicDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Vaccination> Vaccinations { get; set; }
        public DbSet<Feedback> Feedback { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<BlockedSlot> BlockedSlots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all entity configurations
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new PetConfiguration());
            modelBuilder.ApplyConfiguration(new ServiceConfiguration());
            modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
            modelBuilder.ApplyConfiguration(new MedicalRecordConfiguration());
            modelBuilder.ApplyConfiguration(new VaccinationConfiguration());
            modelBuilder.ApplyConfiguration(new FeedbackConfiguration());
            modelBuilder.ApplyConfiguration(new InvoiceConfiguration());
            modelBuilder.ApplyConfiguration(new ReminderConfiguration());
            modelBuilder.ApplyConfiguration(new BlockedSlotConfiguration());
        }
    }
}
