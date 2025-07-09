using Microsoft.EntityFrameworkCore;
using VetClinic.Repository.Entities;
using VetClinic.Repository.Data.Configurations;

namespace VetClinic.Repository.Data
{
    public class VetClinicDbContext : DbContext
    {
        public VetClinicDbContext(DbContextOptions<VetClinicDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());

        }
    }
}
