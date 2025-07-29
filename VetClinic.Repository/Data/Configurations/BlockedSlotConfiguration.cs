using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Data.Configurations
{
    public class BlockedSlotConfiguration : IEntityTypeConfiguration<BlockedSlot>
    {
        public void Configure(EntityTypeBuilder<BlockedSlot> builder)
        {
            builder.HasKey(bs => bs.Id);

            builder.Property(bs => bs.BlockType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(bs => bs.Reason)
                .HasMaxLength(500);

            builder.Property(bs => bs.StartTime)
                .IsRequired();

            builder.Property(bs => bs.EndTime)
                .IsRequired();

            builder.Property(bs => bs.CreatedDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(bs => bs.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Foreign key relationship with User (Doctor)
            builder.HasOne(bs => bs.Doctor)
                .WithMany()
                .HasForeignKey(bs => bs.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index for better query performance
            builder.HasIndex(bs => new { bs.DoctorId, bs.StartTime, bs.EndTime })
                .HasDatabaseName("IX_BlockedSlots_Doctor_TimeRange");

            builder.HasIndex(bs => bs.DoctorId)
                .HasDatabaseName("IX_BlockedSlots_DoctorId");
        }
    }
}
