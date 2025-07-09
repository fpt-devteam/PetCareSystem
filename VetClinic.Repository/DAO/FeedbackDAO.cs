using Microsoft.EntityFrameworkCore;
using VetClinic.Repository.Data;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.DAO
{
    public class FeedbackDAO : BaseDAO<Feedback>, IFeedbackDAO
    {
        public FeedbackDAO(VetClinicDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Feedback>> GetFeedbackByAppointmentAsync(int appointmentId)
        {
            return await _dbSet
                .Where(f => f.AppointmentId == appointmentId)
                .Include(f => f.Customer)
                .OrderByDescending(f => f.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Feedback>> GetApprovedFeedbackAsync()
        {
            return await _dbSet
                .Where(f => f.Approved)
                .Include(f => f.Customer)
                .Include(f => f.Appointment)
                .ThenInclude(a => a.Service)
                .OrderByDescending(f => f.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Feedback>> GetPendingFeedbackAsync()
        {
            return await _dbSet
                .Where(f => !f.Approved)
                .Include(f => f.Customer)
                .Include(f => f.Appointment)
                .ThenInclude(a => a.Service)
                .OrderByDescending(f => f.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Feedback>> GetFeedbackByCustomerAsync(int customerId)
        {
            return await _dbSet
                .Where(f => f.CustomerId == customerId)
                .Include(f => f.Appointment)
                .ThenInclude(a => a.Service)
                .OrderByDescending(f => f.CreatedDate)
                .ToListAsync();
        }

        public async Task<bool> ApproveFeedbackAsync(int feedbackId)
        {
            var feedback = await GetByIdAsync(feedbackId);
            if (feedback == null)
                return false;

            feedback.Approved = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<double> GetAverageRatingAsync()
        {
            return await _dbSet
                .Where(f => f.Approved)
                .AverageAsync(f => (double)f.Rating);
        }

        public async Task<IEnumerable<Feedback>> GetTopRatedFeedbackAsync(int count = 10)
        {
            return await _dbSet
                .Where(f => f.Approved)
                .Include(f => f.Customer)
                .Include(f => f.Appointment)
                .ThenInclude(a => a.Service)
                .OrderByDescending(f => f.Rating)
                .ThenByDescending(f => f.CreatedDate)
                .Take(count)
                .ToListAsync();
        }
    }
}