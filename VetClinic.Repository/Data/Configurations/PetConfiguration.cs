using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Data.Configurations
{
    public class PetConfiguration : IEntityTypeConfiguration<Pet>
    {
        public void Configure(EntityTypeBuilder<Pet> builder)
        {
            // Primary Key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Species)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.Breed)
                .HasMaxLength(100);

            builder.Property(e => e.Weight)
                .HasColumnType("decimal(5,2)");

            builder.Property(e => e.PhotoUrl)
                .HasMaxLength(500);

            builder.Property(e => e.CreatedDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            // Foreign Key
            builder.Property(e => e.OwnerId)
                .IsRequired();

            // Relationships
            builder.HasOne(e => e.Owner)
                .WithMany(u => u.Pets)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("Pets");
        }
    }
}