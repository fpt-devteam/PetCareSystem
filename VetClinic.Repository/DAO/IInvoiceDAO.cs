using VetClinic.Repository.Entities;

namespace VetClinic.Repository.DAO
{
    public interface IInvoiceDAO : IBaseDAO<Invoice>
    {
        Task<Invoice?> GetInvoiceByAppointmentAsync(int appointmentId);
        Task<IEnumerable<Invoice>> GetUnpaidInvoicesAsync();
        Task<IEnumerable<Invoice>> GetOverdueInvoicesAsync();
        Task<IEnumerable<Invoice>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<bool> MarkInvoicePaidAsync(int invoiceId, DateTime paidDate);
        Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Invoice>> GetInvoicesByStatusAsync(string status);
    }
}