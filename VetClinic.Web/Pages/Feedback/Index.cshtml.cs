using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VetClinic.Repository.Entities;
using VetClinic.Service.Interfaces;
using VetClinic.Web.Helpers;

namespace VetClinic.Web.Pages.Feedback
{
    public class IndexModel : PageModel
    {
        private readonly IFeedbackService _feedbackService;

        public IndexModel(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        public IEnumerable<VetClinic.Repository.Entities.Feedback>? AllFeedback { get; set; }
        public IEnumerable<VetClinic.Repository.Entities.Feedback>? PendingFeedback { get; set; }
        public IEnumerable<VetClinic.Repository.Entities.Feedback>? ApprovedFeedback { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
        public string CurrentUserRole { get; set; } = "";

        public async Task<IActionResult> OnGetAsync(int pageNumber = 1)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var userRole = HttpContext.Session.GetString("UserRole");
                // console log for debugging
                System.Console.WriteLine("ahihi");
                Console.WriteLine($"User ID: {userId}, User Role: {userRole}");
                if (!userId.HasValue || string.IsNullOrEmpty(userRole))
                {
                    // console log for debugging
                    Console.WriteLine($"User is not logged {userId} in or role is not set {userRole}.");
                    return RedirectToPage("/Account/Login");
                }

                CurrentUserRole = userRole;
                CurrentPage = pageNumber > 0 ? pageNumber : 1;

                // Convert string role to enum
                if (!Enum.TryParse<Repository.Enums.UserRole>(userRole, out var roleEnum))
                {
                    // console log for debugging
                    Console.WriteLine($"User role is not a valid enum: {userRole}");
                    return RedirectToPage("/Account/Login");
                }

                // Role-based access control
                switch (roleEnum)
                {
                    case Repository.Enums.UserRole.Admin:
                    case Repository.Enums.UserRole.Manager:
                    case Repository.Enums.UserRole.Staff:
                        // Admin, Manager, and Staff see all feedback with pagination
                        var (allFeedback, totalCount) = await _feedbackService.GetPaginatedFeedbackAsync(
                            CurrentPage, PageSize, roleEnum);
                        AllFeedback = allFeedback;
                        TotalPages = (int)Math.Ceiling((double)totalCount / PageSize);

                        // Get all pending feedback (no pagination method in service, so fallback to all)
                        var pendingFeedbackList = await _feedbackService.GetPendingFeedbackAsync();
                        PendingFeedback = pendingFeedbackList;
                        var approvedFeedbackList = await _feedbackService.GetApprovedFeedbackAsync();
                        ApprovedFeedback = approvedFeedbackList;
                        break;

                    case Repository.Enums.UserRole.Doctor:
                        // Doctors see only their reviews with pagination
                        var (doctorFeedback, doctorTotalCount) = await _feedbackService.GetPaginatedFeedbackAsync(
                            CurrentPage, PageSize, roleEnum, userId);
                        AllFeedback = doctorFeedback;
                        TotalPages = (int)Math.Ceiling((double)doctorTotalCount / PageSize);

                        break;

                    case Repository.Enums.UserRole.Customer:
                        // Customers see all feedback with pagination
                        var (customerFeedback, customerTotalCount) = await _feedbackService.GetPaginatedFeedbackAsync(
                            CurrentPage, PageSize, roleEnum);
                        AllFeedback = customerFeedback;
                        TotalPages = (int)Math.Ceiling((double)customerTotalCount / PageSize);
                        break;

                    default:
                        return RedirectToPage("/Account/Login");
                }

                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in IndexModel OnGet: {ex.Message}");
                AllFeedback = new List<VetClinic.Repository.Entities.Feedback>();
                PendingFeedback = new List<VetClinic.Repository.Entities.Feedback>();
                ApprovedFeedback = new List<VetClinic.Repository.Entities.Feedback>();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostApproveAsync(int feedbackId)
        {
            try
            {
                // Check if user is authorized (Admin or Manager)
                var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole != "Admin" && userRole != "Manager")
                {
                    return RedirectToPage("/Account/Login");
                }

                var success = await _feedbackService.ApproveFeedbackAsync(feedbackId);

                if (success)
                {
                    TempData["SuccessMessage"] = "Feedback has been approved successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to approve feedback.";
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in IndexModel OnPostApprove: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while approving the feedback.";
                return RedirectToPage();
            }
        }
    }
}
