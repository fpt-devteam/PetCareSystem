using VetClinic.Repository.DAO;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly IInvoiceDAO _invoiceDAO;

        public InvoiceRepository(IInvoiceDAO invoiceDAO)
        {
            _invoiceDAO = invoiceDAO;
        }

        public async Task<Invoice?> GetByIdAsync(int id)
        {
            return await _invoiceDAO.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            return await _invoiceDAO.GetAllAsync();
        }

        public async Task<Invoice> CreateAsync(Invoice invoice)
        {
            // Business logic validation
            if (invoice.AppointmentId <= 0)
                throw new ArgumentException("Valid appointment ID is required");

            if (invoice.TotalAmount <= 0)
                throw new ArgumentException("Total amount must be greater than zero");

            // Check if invoice already exists for this appointment
            var existingInvoice = await _invoiceDAO.GetInvoiceByAppointmentAsync(invoice.AppointmentId);
            if (existingInvoice != null)
                throw new ArgumentException("Invoice already exists for this appointment");

            invoice.CreatedDate = DateTime.Now;
            invoice.Status = "Pending";
            return await _invoiceDAO.AddAsync(invoice);
        }

        public async Task<Invoice> UpdateAsync(Invoice invoice)
        {
            // Business logic validation
            var existingInvoice = await _invoiceDAO.GetByIdAsync(invoice.Id);
            if (existingInvoice == null)
                throw new ArgumentException("Invoice not found");

            if (invoice.TotalAmount <= 0)
                throw new ArgumentException("Total amount must be greater than zero");

            return await _invoiceDAO.UpdateAsync(invoice);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _invoiceDAO.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _invoiceDAO.ExistsAsync(id);
        }

        public async Task<Invoice?> GetInvoiceByAppointmentAsync(int appointmentId)
        {
            return await _invoiceDAO.GetInvoiceByAppointmentAsync(appointmentId);
        }

        public async Task<IEnumerable<Invoice>> GetUnpaidInvoicesAsync()
        {
            return await _invoiceDAO.GetUnpaidInvoicesAsync();
        }

        public async Task<IEnumerable<Invoice>> GetOverdueInvoicesAsync()
        {
            return await _invoiceDAO.GetOverdueInvoicesAsync();
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _invoiceDAO.GetInvoicesByDateRangeAsync(startDate, endDate);
        }

        public async Task<bool> MarkInvoicePaidAsync(int invoiceId, DateTime paidDate)
        {
            return await _invoiceDAO.MarkInvoicePaidAsync(invoiceId, paidDate);
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate)
        {
            return await _invoiceDAO.GetTotalRevenueAsync(startDate, endDate);
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesByStatusAsync(string status)
        {
            return await _invoiceDAO.GetInvoicesByStatusAsync(status);
        }
    }
}