using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Repositories
{
    public interface IInvoiceRepository
    {
        Task<Invoice?> GetByIdAsync(int id);
        Task<IEnumerable<Invoice>> GetAllAsync();
        Task<Invoice> CreateAsync(Invoice invoice);
        Task<Invoice> UpdateAsync(Invoice invoice);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        // Invoice-specific methods
        Task<Invoice?> GetInvoiceByAppointmentAsync(int appointmentId);
        Task<IEnumerable<Invoice>> GetUnpaidInvoicesAsync();
        Task<IEnumerable<Invoice>> GetOverdueInvoicesAsync();
        Task<IEnumerable<Invoice>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<bool> MarkInvoicePaidAsync(int invoiceId, DateTime paidDate);
        Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Invoice>> GetInvoicesByStatusAsync(string status);
    }
}