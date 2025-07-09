using VetClinic.Repository.Entities;

namespace VetClinic.Service.Interfaces
{
    public interface IInvoiceService
    {
        Task<Invoice?> GetInvoiceByIdAsync(int id);
        Task<IEnumerable<Invoice>> GetAllInvoicesAsync();
        Task<Invoice> CreateInvoiceAsync(Invoice invoice);
        Task<Invoice> UpdateInvoiceAsync(Invoice invoice);
        Task<bool> DeleteInvoiceAsync(int id);
        Task<bool> InvoiceExistsAsync(int id);

        // Invoice-specific business methods
        Task<Invoice?> GetInvoiceByAppointmentAsync(int appointmentId);
        Task<IEnumerable<Invoice>> GetUnpaidInvoicesAsync();
        Task<IEnumerable<Invoice>> GetOverdueInvoicesAsync();
        Task<IEnumerable<Invoice>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<bool> MarkInvoicePaidAsync(int invoiceId, DateTime paidDate);
        Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Invoice>> GetInvoicesByStatusAsync(string status);
        Task<bool> CanUserAccessInvoiceAsync(int userId, int invoiceId, string userRole);
        Task<Invoice> GenerateInvoiceFromAppointmentAsync(int appointmentId);
    }
}