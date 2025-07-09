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
            services.AddScoped<IPetDAO, PetDAO>();
            services.AddScoped<IServiceDAO, ServiceDAO>();
            services.AddScoped<IAppointmentDAO, AppointmentDAO>();
            services.AddScoped<IMedicalRecordDAO, MedicalRecordDAO>();
            services.AddScoped<IVaccinationDAO, VaccinationDAO>();
            services.AddScoped<IFeedbackDAO, FeedbackDAO>();
            services.AddScoped<IInvoiceDAO, InvoiceDAO>();
            services.AddScoped<IReminderDAO, ReminderDAO>();

            // Register Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPetRepository, PetRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
            services.AddScoped<IVaccinationRepository, VaccinationRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IReminderRepository, ReminderRepository>();

            return services;
        }
    }
}
