using VetClinic.Repository.Entities;

namespace VetClinic.Service.Interfaces
{
    public interface IFeedbackService
    {
        Task<Feedback?> GetFeedbackByIdAsync(int id);
        Task<IEnumerable<Feedback>> GetAllFeedbackAsync();
        Task<Feedback> CreateFeedbackAsync(Feedback feedback);
        Task<Feedback> UpdateFeedbackAsync(Feedback feedback);
        Task<bool> DeleteFeedbackAsync(int id);
        Task<bool> FeedbackExistsAsync(int id);

        // Feedback-specific business methods
        Task<IEnumerable<Feedback>> GetFeedbackByAppointmentAsync(int appointmentId);
        Task<IEnumerable<Feedback>> GetApprovedFeedbackAsync();
        Task<IEnumerable<Feedback>> GetPendingFeedbackAsync();
        Task<IEnumerable<Feedback>> GetFeedbackByCustomerAsync(int customerId);
        Task<bool> ApproveFeedbackAsync(int feedbackId);
        Task<double> GetAverageRatingAsync();
        Task<IEnumerable<Feedback>> GetTopRatedFeedbackAsync(int count = 10);
        Task<bool> CanUserLeaveFeedbackAsync(int userId, int appointmentId);
        Task<bool> CanUserAccessFeedbackAsync(int userId, int feedbackId, string userRole);
    }
}