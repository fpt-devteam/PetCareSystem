using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Data.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            // Primary Key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.TotalAmount)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.CreatedDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            // Foreign Key
            builder.Property(e => e.AppointmentId)
                .IsRequired();

            // Relationships
            builder.HasOne(e => e.Appointment)
                .WithOne(a => a.Invoice)
                .HasForeignKey<Invoice>(e => e.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("Invoices");
        }
    }
}