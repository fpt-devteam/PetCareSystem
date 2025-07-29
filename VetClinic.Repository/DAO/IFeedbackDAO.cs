using VetClinic.Repository.Entities;

namespace VetClinic.Repository.DAO
{
    public interface IFeedbackDAO : IBaseDAO<Feedback>
    {
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