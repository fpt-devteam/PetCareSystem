using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Data.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            // Primary Key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.AppointmentTime)
                .IsRequired();

            builder.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.Notes)
                .HasColumnType("ntext");

            builder.Property(e => e.CreatedDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            // Foreign Keys
            builder.Property(e => e.PetId)
                .IsRequired();

            builder.Property(e => e.DoctorId)
                .IsRequired();

            builder.Property(e => e.ServiceId)
                .IsRequired();

            // Relationships
            builder.HasOne(e => e.Pet)
                .WithMany(p => p.Appointments)
                .HasForeignKey(e => e.PetId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Doctor)
                .WithMany(u => u.DoctorAppointments)
                .HasForeignKey(e => e.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Service)
                .WithMany(s => s.Appointments)
                .HasForeignKey(e => e.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Appointments");
        }
    }
}