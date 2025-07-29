namespace VetClinic.Service.Interfaces
{
    public interface IDashboardService
    {
        // Dashboard analytics methods
        Task<int> GetTotalPetsAsync();
        Task<int> GetTotalAppointmentsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<int> GetTotalCustomersAsync();
        Task<int> GetTotalDoctorsAsync();
        Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<double> GetAverageRatingAsync();

        // Daily summary
        Task<object> GetDailySummaryAsync(DateTime date);
        Task<IEnumerable<object>> GetAppointmentsByDayAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<object>> GetRevenueByMonthAsync(int year);

        // Due items
        Task<int> GetDueVaccinationsCountAsync();
        Task<int> GetOverdueVaccinationsCountAsync();
        Task<int> GetUpcomingAppointmentsCountAsync(int days = 7);
        Task<int> GetUnpaidInvoicesCountAsync();

        // Role-specific dashboards
        Task<object> GetCustomerDashboardAsync(int customerId);
        Task<object> GetDoctorDashboardAsync(int doctorId);
        Task<object> GetManagerDashboardAsync();
        Task<object> GetStaffDashboardAsync();

        // USS-20: Doctor Monthly Performance
        Task<object> GetDoctorMonthlyPerformanceAsync(int doctorId, int year, int month);

        // USS-19: Pet Health Timeline Chart
        Task<object> GetPetHealthTimelineAsync(int petId, DateTime? startDate = null, DateTime? endDate = null);

        // Additional methods for chart data
        Task<object> GetMonthlyRevenueAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<object> GetAppointmentStatusAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<object> GetPetSpeciesDistributionAsync();
        Task<object> GetTopServicesAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}