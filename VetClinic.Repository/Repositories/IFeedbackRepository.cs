using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Repositories
{
    public interface IFeedbackRepository
    {
        Task<Feedback?> GetByIdAsync(int id);
        Task<IEnumerable<Feedback>> GetAllAsync();
        Task<Feedback> CreateAsync(Feedback feedback);
        Task<Feedback> UpdateAsync(Feedback feedback);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        // Feedback-specific methods
        Task<IEnumerable<Feedback>> GetFeedbackByAppointmentAsync(int appointmentId);
        Task<IEnumerable<Feedback>> GetApprovedFeedbackAsync();
        Task<IEnumerable<Feedback>> GetPendingFeedbackAsync();
        Task<IEnumerable<Feedback>> GetFeedbackByCustomerAsync(int customerId);
        Task<bool> ApproveFeedbackAsync(int feedbackId);
        Task<double> GetAverageRatingAsync();
        Task<IEnumerable<Feedback>> GetTopRatedFeedbackAsync(int count = 10);

        Task<IEnumerable<Feedback>> GetFeedbackByDoctorAsync(int doctorId);
    }
}