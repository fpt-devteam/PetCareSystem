using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Data.Configurations
{
    public class VaccinationConfiguration : IEntityTypeConfiguration<Vaccination>
    {
        public void Configure(EntityTypeBuilder<Vaccination> builder)
        {
            // Primary Key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.VaccineName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.DueDate)
                .IsRequired();

            builder.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.Notes)
                .HasColumnType("ntext");

            builder.Property(e => e.CreatedDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            // Foreign Key
            builder.Property(e => e.PetId)
                .IsRequired();

            // Relationships
            builder.HasOne(e => e.Pet)
                .WithMany(p => p.Vaccinations)
                .HasForeignKey(e => e.PetId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("Vaccinations");
        }
    }
}