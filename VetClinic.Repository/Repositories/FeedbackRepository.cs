using VetClinic.Repository.DAO;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly IFeedbackDAO _feedbackDAO;

        public FeedbackRepository(IFeedbackDAO feedbackDAO)
        {
            _feedbackDAO = feedbackDAO;
        }

        public async Task<Feedback?> GetByIdAsync(int id)
        {
            return await _feedbackDAO.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Feedback>> GetAllAsync()
        {
            return await _feedbackDAO.GetAllAsync();
        }

        public async Task<Feedback> CreateAsync(Feedback feedback)
        {
            // Business logic validation
            if (feedback.AppointmentId <= 0)
                throw new ArgumentException("Valid appointment ID is required");

            if (feedback.CustomerId <= 0)
                throw new ArgumentException("Valid customer ID is required");

            if (feedback.Rating < 1 || feedback.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            // Check if feedback already exists for this appointment by this customer
            var existingFeedback = await _feedbackDAO.GetFeedbackByAppointmentAsync(feedback.AppointmentId);
            if (existingFeedback.Any(f => f.CustomerId == feedback.CustomerId))
                throw new ArgumentException("Feedback already exists for this appointment");

            feedback.CreatedDate = DateTime.Now;
            feedback.Approved = false; // New feedback needs approval
            return await _feedbackDAO.AddAsync(feedback);
        }

        public async Task<Feedback> UpdateAsync(Feedback feedback)
        {
            // Business logic validation
            var existingFeedback = await _feedbackDAO.GetByIdAsync(feedback.Id);
            if (existingFeedback == null)
                throw new ArgumentException("Feedback not found");

            if (feedback.Rating < 1 || feedback.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            return await _feedbackDAO.UpdateAsync(feedback);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _feedbackDAO.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _feedbackDAO.ExistsAsync(id);
        }

        public async Task<IEnumerable<Feedback>> GetFeedbackByAppointmentAsync(int appointmentId)
        {
            return await _feedbackDAO.GetFeedbackByAppointmentAsync(appointmentId);
        }

        public async Task<IEnumerable<Feedback>> GetApprovedFeedbackAsync()
        {
            return await _feedbackDAO.GetApprovedFeedbackAsync();
        }

        public async Task<IEnumerable<Feedback>> GetPendingFeedbackAsync()
        {
            return await _feedbackDAO.GetPendingFeedbackAsync();
        }

        public async Task<IEnumerable<Feedback>> GetFeedbackByCustomerAsync(int customerId)
        {
            return await _feedbackDAO.GetFeedbackByCustomerAsync(customerId);
        }

        public async Task<bool> ApproveFeedbackAsync(int feedbackId)
        {
            return await _feedbackDAO.ApproveFeedbackAsync(feedbackId);
        }

        public async Task<double> GetAverageRatingAsync()
        {
            return await _feedbackDAO.GetAverageRatingAsync();
        }

        public async Task<IEnumerable<Feedback>> GetTopRatedFeedbackAsync(int count = 10)
        {
            return await _feedbackDAO.GetTopRatedFeedbackAsync(count);
        }

        public async Task<IEnumerable<Feedback>> GetFeedbackByDoctorAsync(int doctorId)
        {
            return await _feedbackDAO.GetFeedbackByDoctorAsync(doctorId);
        }
    }
}