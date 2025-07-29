using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Data.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.UserRole)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.Action)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.EntityType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Description)
                .HasMaxLength(500);

            builder.Property(a => a.IpAddress)
                .HasMaxLength(45);

            builder.Property(a => a.UserAgent)
                .HasMaxLength(500);

            builder.Property(a => a.AdditionalData)
                .HasMaxLength(1000);

            builder.Property(a => a.Timestamp)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            // Relationships
            builder.HasOne(a => a.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes for performance
            builder.HasIndex(a => a.UserId);
            builder.HasIndex(a => a.Timestamp);
            builder.HasIndex(a => a.EntityType);
            builder.HasIndex(a => a.Action);
        }
    }
}
