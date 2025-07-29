using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using VetClinic.Repository.Entities;
using VetClinic.Service.Interfaces;
using VetClinic.Web.Helpers;

namespace VetClinic.Web.Pages.Appointments
{
    public class ManageModel : PageModel
    {
        private readonly IAppointmentService _appointmentService;

        public ManageModel(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        public IEnumerable<Appointment> Appointments { get; set; } = new List<Appointment>();

        // Statistics
        public int UpcomingCount { get; set; }
        public int CompletedCount { get; set; }
        public int PendingCount { get; set; }
        public int CancelledCount { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            var userRole = SessionHelper.GetUserRole(HttpContext.Session);
            if (userRole != "Customer")
            {
                TempData["ErrorMessage"] = "Only customers can manage their appointments.";
                return RedirectToPage("/Appointments/Index");
            }

            try
            {
                await LoadAppointmentsAsync();
                LoadStatistics();
                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading your appointments. Please try again.";
                Console.WriteLine($"Error loading customer appointments: {ex.Message}");
                return Page();
            }
        }

        public async Task<IActionResult> OnPostRescheduleAsync(int appointmentId, DateTime newAppointmentTime)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                var userId = SessionHelper.GetUserId(HttpContext.Session);
                var userRole = SessionHelper.GetUserRole(HttpContext.Session);

                if (!userId.HasValue || userRole != "Customer")
                {
                    TempData["ErrorMessage"] = "Unable to verify user session.";
                    return RedirectToPage();
                }

                // Validate new appointment time
                if (newAppointmentTime <= DateTime.Now.AddHours(24))
                {
                    TempData["ErrorMessage"] = "Appointments can only be rescheduled at least 24 hours in advance.";
                    return RedirectToPage();
                }

                // Check if it's during business hours (8 AM - 6 PM, Monday-Saturday)
                if (newAppointmentTime.Hour < 8 || newAppointmentTime.Hour >= 18)
                {
                    TempData["ErrorMessage"] = "Appointments can only be scheduled between 8 AM and 6 PM.";
                    return RedirectToPage();
                }

                if (newAppointmentTime.DayOfWeek == DayOfWeek.Sunday)
                {
                    TempData["ErrorMessage"] = "Appointments cannot be scheduled on Sundays.";
                    return RedirectToPage();
                }

                var success = await _appointmentService.RescheduleAppointmentAsync(appointmentId, newAppointmentTime, userId.Value, userRole);

                if (success)
                {
                    TempData["SuccessMessage"] = "Your appointment has been rescheduled successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to reschedule your appointment. The doctor may not be available at the selected time.";
                }
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error rescheduling appointment. Please try again.";
                Console.WriteLine($"Error rescheduling appointment: {ex.Message}");
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCancelAsync(int appointmentId)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                var userId = SessionHelper.GetUserId(HttpContext.Session);
                var userRole = SessionHelper.GetUserRole(HttpContext.Session);

                if (!userId.HasValue || userRole != "Customer")
                {
                    TempData["ErrorMessage"] = "Unable to verify user session.";
                    return RedirectToPage();
                }

                var success = await _appointmentService.CancelAppointmentAsync(appointmentId, userId.Value, userRole);

                if (success)
                {
                    TempData["SuccessMessage"] = "Your appointment has been cancelled successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to cancel your appointment. It may be too close to the appointment time or already completed.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error cancelling appointment. Please try again.";
                Console.WriteLine($"Error cancelling appointment: {ex.Message}");
            }

            return RedirectToPage();
        }

        private async Task LoadAppointmentsAsync()
        {
            var userId = SessionHelper.GetUserId(HttpContext.Session);
            if (!userId.HasValue) return;

            try
            {
                var appointments = await _appointmentService.GetAppointmentsByOwnerAsync(userId.Value);
                Appointments = appointments.OrderBy(a => a.AppointmentTime).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading appointments: {ex.Message}");
                Appointments = new List<Appointment>();
            }
        }

        private void LoadStatistics()
        {
            var today = DateTime.Today;
            var appointmentsList = Appointments.ToList();

            UpcomingCount = appointmentsList.Count(a => 
                a.AppointmentTime.Date >= today && 
                (a.Status == "Scheduled" || a.Status == "Confirmed"));

            CompletedCount = appointmentsList.Count(a => a.Status == "Completed");
            PendingCount = appointmentsList.Count(a => a.Status == "Scheduled" || a.Status == "Pending");
            CancelledCount = appointmentsList.Count(a => a.Status == "Cancelled");
        }
    }
}
