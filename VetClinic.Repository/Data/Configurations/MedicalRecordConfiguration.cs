using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Data.Configurations
{
    public class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
    {
        public void Configure(EntityTypeBuilder<MedicalRecord> builder)
        {
            // Primary Key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.VisitDate)
                .IsRequired();

            builder.Property(e => e.Diagnosis)
                .HasColumnType("ntext");

            builder.Property(e => e.TreatmentNotes)
                .HasColumnType("ntext");

            builder.Property(e => e.Prescription)
                .HasColumnType("ntext");

            builder.Property(e => e.CreatedDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            // Foreign Keys
            builder.Property(e => e.AppointmentId)
                .IsRequired();

            builder.Property(e => e.PetId)
                .IsRequired();

            builder.Property(e => e.DoctorId)
                .IsRequired();

            // Relationships
            builder.HasOne(e => e.Appointment)
                .WithOne(a => a.MedicalRecord)
                .HasForeignKey<MedicalRecord>(e => e.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Pet)
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(e => e.PetId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Doctor)
                .WithMany(u => u.DoctorMedicalRecords)
                .HasForeignKey(e => e.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("MedicalRecords");
        }
    }
}