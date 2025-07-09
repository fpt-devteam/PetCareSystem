using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VetClinic.Repository.Data;
using VetClinic.Repository.DAO;
using VetClinic.Repository.Repositories;

namespace VetClinic.Repository
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepository(this IServiceCollection services, IConfiguration configuration)
        {
            // Add DbContext
            services.AddDbContext<VetClinicDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Register DAOs
            services.AddScoped<IUserDAO, UserDAO>();

            // Register Repositories
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
