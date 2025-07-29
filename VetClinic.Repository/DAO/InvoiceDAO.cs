using Microsoft.EntityFrameworkCore;
using VetClinic.Repository.Data;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.DAO
{
    public class InvoiceDAO : BaseDAO<Invoice>, IInvoiceDAO
    {
        public InvoiceDAO(VetClinicDbContext context) : base(context)
        {
        }

        public async Task<Invoice?> GetInvoiceByAppointmentAsync(int appointmentId)
        {
            return await _dbSet
                .Include(i => i.Appointment)
                .ThenInclude(a => a.Pet)
                .ThenInclude(p => p.Owner)
                .Include(i => i.Appointment)
                .ThenInclude(a => a.Service)
                .FirstOrDefaultAsync(i => i.AppointmentId == appointmentId);
        }

        public async Task<IEnumerable<Invoice>> GetUnpaidInvoicesAsync()
        {
            return await _dbSet
                .Where(i => i.Status == "Pending")
                .Include(i => i.Appointment)
                .ThenInclude(a => a.Pet)
                .ThenInclude(p => p.Owner)
                .OrderBy(i => i.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetOverdueInvoicesAsync()
        {
            var overdueDate = DateTime.Today.AddDays(-30); // 30 days overdue
            return await _dbSet
                .Where(i => i.Status == "Pending" && i.CreatedDate < overdueDate)
                .Include(i => i.Appointment)
                .ThenInclude(a => a.Pet)
                .ThenInclude(p => p.Owner)
                .OrderBy(i => i.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(i => i.CreatedDate >= startDate && i.CreatedDate <= endDate)
                .Include(i => i.Appointment)
                .ThenInclude(a => a.Pet)
                .ThenInclude(p => p.Owner)
                .OrderByDescending(i => i.CreatedDate)
                .ToListAsync();
        }

        public async Task<bool> MarkInvoicePaidAsync(int invoiceId, DateTime paidDate)
        {
            var invoice = await GetByIdAsync(invoiceId);
            if (invoice == null)
                return false;

            invoice.Status = "Paid";
            invoice.PaidDate = paidDate;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(i => i.Status == "Paid" && i.PaidDate >= startDate && i.PaidDate <= endDate)
                .SumAsync(i => i.TotalAmount);
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesByStatusAsync(string status)
        {
            return await _dbSet
                .Where(i => i.Status == status)
                .Include(i => i.Appointment)
                .ThenInclude(a => a.Pet)
                .ThenInclude(p => p.Owner)
                .OrderByDescending(i => i.CreatedDate)
                .ToListAsync();
        }
    }
}