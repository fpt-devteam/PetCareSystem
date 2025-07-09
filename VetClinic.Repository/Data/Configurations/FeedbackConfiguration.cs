using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Data.Configurations
{
    public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
            // Primary Key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.Rating)
                .IsRequired();

            builder.Property(e => e.Comment)
                .HasColumnType("ntext");

            builder.Property(e => e.Approved)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.CreatedDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            // Foreign Keys
            builder.Property(e => e.AppointmentId)
                .IsRequired();

            builder.Property(e => e.CustomerId)
                .IsRequired();

            // Relationships
            builder.HasOne(e => e.Appointment)
                .WithMany(a => a.Feedback)
                .HasForeignKey(e => e.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Customer)
                .WithMany(u => u.CustomerFeedback)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Feedback");
        }
    }
}