using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VetClinic.Repository.Entities;
using VetClinic.Service.Interfaces;
using VetClinic.Web.Helpers;

namespace VetClinic.Web.Pages.Feedback
{
    public class MyReviewsModel : PageModel
    {
        private readonly IFeedbackService _feedbackService;

        public MyReviewsModel(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        public IEnumerable<VetClinic.Repository.Entities.Feedback> MyFeedback { get; set; } = new List<VetClinic.Repository.Entities.Feedback>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 9; // 3 columns × 3 rows for better layout
        public int TotalReviews { get; set; }

        public async Task<IActionResult> OnGetAsync(int pageNumber = 1)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");
            // console log for debugging
            Console.WriteLine($"User ID: {userId}, User Role: {userRole}");

            if (!userId.HasValue || string.IsNullOrEmpty(userRole))
            {
                // console log for debugging
                Console.WriteLine($"User is not logged {userId} in or role is not set {userRole}.");
                return RedirectToPage("/Account/Login");
            }

            if (!Enum.TryParse<Repository.Enums.UserRole>(userRole, out var roleEnum))
            {
                // console log for debugging
                Console.WriteLine($"User role is not a valid enum: {userRole}");
                return RedirectToPage("/Account/Login");
            }

            CurrentPage = pageNumber > 0 ? pageNumber : 1;

            var (feedback, totalCount) = await _feedbackService.GetPaginatedFeedbackAsync(
                CurrentPage, PageSize, roleEnum, userId);

            MyFeedback = feedback;
            TotalReviews = totalCount;
            TotalPages = (int)Math.Ceiling((double)totalCount / PageSize);

            return Page();
        }
    }
}
