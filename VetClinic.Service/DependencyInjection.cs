using Microsoft.Extensions.DependencyInjection;
using VetClinic.Service.Interfaces;
using VetClinic.Service.Services;

namespace VetClinic.Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Register Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
