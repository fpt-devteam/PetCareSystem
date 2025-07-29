using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using VetClinic.Repository.Entities;
using VetClinic.Service.Interfaces;

namespace VetClinic.Web.Pages.Feedback
{
    public class CreateModel : PageModel
    {
        private readonly IFeedbackService _feedbackService;
        private readonly IAppointmentService _appointmentService;

        public CreateModel(IFeedbackService feedbackService, IAppointmentService appointmentService)
        {
            _feedbackService = feedbackService;
            _appointmentService = appointmentService;
        }

        [BindProperty]
        public int AppointmentId { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please select a rating")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars")]
        public int Rating { get; set; }

        [BindProperty]
        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        public string? Comment { get; set; }

        public Appointment? Appointment { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int appointmentId)
        {
            try
            {
                AppointmentId = appointmentId;
                Console.WriteLine($"Loading feedback form for appointment {AppointmentId}");

                // Get current user from session
                var userId = HttpContext.Session.GetInt32("UserId");
                var userRole = HttpContext.Session.GetString("UserRole");

                if (!userId.HasValue || userRole != "Customer")
                {
                    return RedirectToPage("/Account/Login");
                }

                // Check if user can leave feedback for this appointment
                var canLeaveFeedback = await _feedbackService.CanUserLeaveFeedbackAsync(userId.Value, appointmentId);
                if (!canLeaveFeedback)
                {
                    ErrorMessage = "You cannot leave feedback for this appointment. " +
                                 "You can only rate completed appointments for your own pets, and you can only rate each appointment once.";
                    return Page();
                }

                // Get appointment details
                Appointment = await _appointmentService.GetAppointmentByIdAsync(appointmentId);
                if (Appointment == null)
                {
                    ErrorMessage = "Appointment not found.";
                    return Page();
                }

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while loading the feedback form.";
                Console.WriteLine($"Error in CreateModel OnGet: {ex.Message}");
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Reload appointment details if validation fails
                    Appointment = await _appointmentService.GetAppointmentByIdAsync(AppointmentId);
                    return Page();
                }

                // Get current user from session
                var userId = HttpContext.Session.GetInt32("UserId");
                var userRole = HttpContext.Session.GetString("UserRole");

                if (!userId.HasValue || userRole != "Customer")
                {
                    return RedirectToPage("/Account/Login");
                }

                // Check if user can leave feedback for this appointment
                var canLeaveFeedback = await _feedbackService.CanUserLeaveFeedbackAsync(userId.Value, AppointmentId);
                if (!canLeaveFeedback)
                {
                    ErrorMessage = "You cannot leave feedback for this appointment.";
                    Appointment = await _appointmentService.GetAppointmentByIdAsync(AppointmentId);
                    return Page();
                }

                // Create feedback entity
                var feedback = new VetClinic.Repository.Entities.Feedback
                {
                    AppointmentId = AppointmentId,
                    CustomerId = userId.Value,
                    Rating = Rating,
                    Comment = Comment?.Trim(),
                    CreatedDate = DateTime.Now,
                    Approved = false // Will need admin approval
                };

                // Save feedback
                await _feedbackService.CreateFeedbackAsync(feedback);

                // Set success message in TempData
                TempData["SuccessMessage"] = "Thank you for your feedback! Your review has been submitted and will be published after approval.";

                // Redirect to appointments list
                return RedirectToPage("/Appointments/Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.Contains("already exists")
                    ? "You have already provided feedback for this appointment."
                    : "An error occurred while submitting your feedback. Please try again.";

                Console.WriteLine($"Error in CreateModel OnPost: {ex.Message}");

                // Reload appointment details
                Appointment = await _appointmentService.GetAppointmentByIdAsync(AppointmentId);
                return Page();
            }
        }
    }
}
