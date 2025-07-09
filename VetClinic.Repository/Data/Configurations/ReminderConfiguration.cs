using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Data.Configurations
{
    public class ReminderConfiguration : IEntityTypeConfiguration<Reminder>
    {
        public void Configure(EntityTypeBuilder<Reminder> builder)
        {
            // Primary Key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.Message)
                .HasColumnType("ntext");

            builder.Property(e => e.SendAt)
                .IsRequired();

            builder.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.CreatedDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            // Foreign Keys
            builder.Property(e => e.UserId)
                .IsRequired();

            // Relationships
            builder.HasOne(e => e.User)
                .WithMany(u => u.Reminders)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Pet)
                .WithMany()
                .HasForeignKey(e => e.PetId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Reminders");
        }
    }
}