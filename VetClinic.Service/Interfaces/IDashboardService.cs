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
    }
}