using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Data.Configurations
{
    public class LabResultConfiguration : IEntityTypeConfiguration<LabResult>
    {
        public void Configure(EntityTypeBuilder<LabResult> builder)
        {
            builder.HasKey(l => l.Id);

            builder.Property(l => l.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(l => l.FileType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(l => l.FilePath)
                .HasMaxLength(500);

            builder.Property(l => l.ContentType)
                .HasMaxLength(255);

            builder.Property(l => l.FileContent)
                .HasColumnType("varbinary(max)");

            builder.Property(l => l.Description)
                .HasMaxLength(500);

            builder.Property(l => l.LabName)
                .HasMaxLength(100);

            builder.Property(l => l.UploadDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(l => l.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Relationships
            builder.HasOne(l => l.MedicalRecord)
                .WithMany(m => m.LabResults)
                .HasForeignKey(l => l.MedicalRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(l => l.UploadedByUser)
                .WithMany(u => u.UploadedLabResults)
                .HasForeignKey(l => l.UploadedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes for performance
            builder.HasIndex(l => l.MedicalRecordId);
            builder.HasIndex(l => l.UploadedBy);
            builder.HasIndex(l => l.UploadDate);
            builder.HasIndex(l => l.IsActive);
        }
    }
}
