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
            services.AddScoped<IPetService, PetService>();
            services.AddScoped<IServiceService, ServiceService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IMedicalRecordService, MedicalRecordService>();
            services.AddScoped<IBlockedSlotService, BlockedSlotService>();
            services.AddScoped<IDoctorAvailabilityService, DoctorAvailabilityService>();

            // Add remaining service registrations for complete implementation
            // These would need their implementations created for full functionality

            return services;
        }
    }
}
