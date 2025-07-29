using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VetClinic.Repository.Entities;
using VetClinic.Service.Interfaces;
using VetClinic.Web.Helpers;

namespace VetClinic.Web.Pages.Staff
{
    public class AgendaModel : PageModel
    {
        private readonly IAppointmentService _appointmentService;

        public AgendaModel(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        public IEnumerable<Appointment> TodayAppointments { get; set; } = new List<Appointment>();

        // Statistics
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int InProgressAppointments { get; set; }
        public int UpcomingAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int UniqueDoctors { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            var userRole = SessionHelper.GetUserRole(HttpContext.Session);
            if (userRole != "Staff" && userRole != "Admin" && userRole != "Manager")
            {
                TempData["ErrorMessage"] = "Access denied. This page is for staff members only.";
                return RedirectToPage("/");
            }

            try
            {
                await LoadTodayAppointmentsAsync();
                LoadStatistics();
                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading today's agenda. Please try again.";
                Console.WriteLine($"Error loading daily agenda: {ex.Message}");
                return Page();
            }
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int appointmentId, string newStatus)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            var userRole = SessionHelper.GetUserRole(HttpContext.Session);
            if (userRole != "Staff" && userRole != "Admin" && userRole != "Manager")
            {
                TempData["ErrorMessage"] = "Access denied.";
                return RedirectToPage();
            }

            try
            {
                var userId = SessionHelper.GetUserId(HttpContext.Session);
                if (!userId.HasValue)
                {
                    TempData["ErrorMessage"] = "Unable to verify user session.";
                    return RedirectToPage();
                }

                var success = await _appointmentService.UpdateAppointmentStatusAsync(appointmentId, newStatus, userId.Value);

                if (success)
                {
                    TempData["SuccessMessage"] = $"Appointment status updated to {newStatus}.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to update appointment status.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error updating appointment status.";
                Console.WriteLine($"Error updating appointment status: {ex.Message}");
            }

            return RedirectToPage();
        }

        private async Task LoadTodayAppointmentsAsync()
        {
            try
            {
                var today = DateTime.Today;
                var appointments = await _appointmentService.GetDailyAgendaAsync(today);
                TodayAppointments = appointments
                    .OrderBy(a => a.AppointmentTime)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading today's appointments: {ex.Message}");
                TodayAppointments = new List<Appointment>();
            }
        }

        private void LoadStatistics()
        {
            var appointmentsList = TodayAppointments.ToList();

            TotalAppointments = appointmentsList.Count;
            CompletedAppointments = appointmentsList.Count(a => a.Status == "Completed");
            InProgressAppointments = appointmentsList.Count(a => a.Status == "InProgress");
            UpcomingAppointments = appointmentsList.Count(a => 
                a.AppointmentTime > DateTime.Now && 
                (a.Status == "Scheduled" || a.Status == "Confirmed"));
            CancelledAppointments = appointmentsList.Count(a => a.Status == "Cancelled");
            UniqueDoctors = appointmentsList
                .Where(a => a.Status != "Cancelled")
                .Select(a => a.DoctorId)
                .Distinct()
                .Count();
        }
    }
}
