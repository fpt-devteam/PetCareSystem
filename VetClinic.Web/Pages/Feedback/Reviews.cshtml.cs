using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VetClinic.Repository.Entities;
using VetClinic.Service.Interfaces;
using VetClinic.Web.Helpers;

namespace VetClinic.Web.Pages.Feedback
{
    public class ReviewsModel : PageModel
    {
        private readonly IFeedbackService _feedbackService;

        public ReviewsModel(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        public IEnumerable<VetClinic.Repository.Entities.Feedback> ApprovedFeedback { get; set; } = new List<VetClinic.Repository.Entities.Feedback>();
        public double AverageRating { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }

        public async Task OnGetAsync(int pageNumber = 1)
        {
            try
            {
                CurrentPage = pageNumber > 0 ? pageNumber : 1;

                var result = await _feedbackService.GetPaginatedApprovedFeedbackAsync(CurrentPage, PageSize);
                ApprovedFeedback = result.Feedbacks;
                TotalCount = result.TotalCount;
                TotalPages = (int)Math.Ceiling((double)TotalCount / PageSize);

                AverageRating = await _feedbackService.GetAverageRatingAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ReviewsModel OnGet: {ex.Message}");
                ApprovedFeedback = new List<VetClinic.Repository.Entities.Feedback>();
                AverageRating = 0.0;
                TotalCount = 0;
                TotalPages = 0;
            }
        }
    }
}
