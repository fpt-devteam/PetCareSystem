using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VetClinic.Repository.Entities;
using VetClinic.Service.Interfaces;
using VetClinic.Web.Helpers;

namespace VetClinic.Web.Pages.Appointments
{
    public class IndexModel : PageModel
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IUserService _userService;

        public IndexModel(IAppointmentService appointmentService, IUserService userService)
        {
            _appointmentService = appointmentService;
            _userService = userService;
        }

        public IEnumerable<Appointment> Appointments { get; set; } = new List<Appointment>();
        public SelectList? DoctorSelectList { get; set; }

        // Filter properties
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? DateFrom { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? DateTo { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? DoctorFilter { get; set; }

        // Stats properties
        public int TodayCount { get; set; }
        public int UpcomingCount { get; set; }
        public int PendingCount { get; set; }
        public int CompletedCount { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                await LoadDataAsync();
                await LoadStatsAsync();
                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading appointments. Please try again.";
                Console.WriteLine($"Error loading appointments: {ex.Message}");
                return Page();
            }
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int appointmentId, string newStatus)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                var userId = SessionHelper.GetUserId(HttpContext.Session);
                if (!userId.HasValue)
                {
                    TempData["ErrorMessage"] = "Unable to verify user session.";
                    return RedirectToPage();
                }

                await _appointmentService.UpdateAppointmentStatusAsync(appointmentId, newStatus, userId.Value);
                TempData["SuccessMessage"] = $"Appointment status updated to {newStatus}.";
            }
            catch (UnauthorizedAccessException)
            {
                TempData["ErrorMessage"] = "You are not authorized to update this appointment.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error updating appointment status.";
                Console.WriteLine($"Error updating appointment status: {ex.Message}");
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
                if (!userId.HasValue)
                {
                    TempData["ErrorMessage"] = "Unable to verify user session.";
                    return RedirectToPage();
                }

                var userRole = SessionHelper.GetUserRole(HttpContext.Session) ?? "";
                await _appointmentService.CancelAppointmentAsync(appointmentId, userId.Value, userRole);
                TempData["SuccessMessage"] = "Appointment has been cancelled.";
            }
            catch (UnauthorizedAccessException)
            {
                TempData["ErrorMessage"] = "You are not authorized to cancel this appointment.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error cancelling appointment.";
                Console.WriteLine($"Error cancelling appointment: {ex.Message}");
            }

            return RedirectToPage();
        }

        private async Task LoadDataAsync()
        {
            var userId = SessionHelper.GetUserId(HttpContext.Session);
            var userRole = SessionHelper.GetUserRole(HttpContext.Session);

            if (!userId.HasValue)
            {
                Appointments = new List<Appointment>();
                return;
            }

            // Load appointments based on user role
            if (userRole == "Customer")
            {
                // Customers see only their pets' appointments
                var pets = await _appointmentService.GetAppointmentsByOwnerAsync(userId.Value);
                Appointments = pets;
            }
            else if (userRole == "Doctor")
            {
                // Doctors see their assigned appointments (for today and future)
                var doctorAppointments = await _appointmentService.GetAppointmentsByDoctorAsync(userId.Value, DateTime.Today);
                Appointments = doctorAppointments;
            }
            else
            {
                // Admin, Manager, Staff see all appointments
                var allAppointments = await _appointmentService.GetAllAppointmentsAsync();
                Appointments = allAppointments;
            }

            // Apply filters
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                var searchLower = SearchTerm.ToLower();
                Appointments = Appointments.Where(a =>
                    (a.Pet?.Name?.ToLower().Contains(searchLower) ?? false) ||
                    (a.Pet?.Owner?.FullName?.ToLower().Contains(searchLower) ?? false) ||
                    (a.Notes?.ToLower().Contains(searchLower) ?? false) ||
                    (a.Service?.Name?.ToLower().Contains(searchLower) ?? false));
            }

            if (!string.IsNullOrEmpty(StatusFilter))
            {
                Appointments = Appointments.Where(a => a.Status == StatusFilter);
            }

            if (DateFrom.HasValue)
            {
                Appointments = Appointments.Where(a => a.AppointmentTime.Date >= DateFrom.Value.Date);
            }

            if (DateTo.HasValue)
            {
                Appointments = Appointments.Where(a => a.AppointmentTime.Date <= DateTo.Value.Date);
            }

            if (DoctorFilter.HasValue && DoctorFilter > 0)
            {
                Appointments = Appointments.Where(a => a.DoctorId == DoctorFilter);
            }

            // Sort by appointment time
            Appointments = Appointments.OrderBy(a => a.AppointmentTime).ToList();

            // Load doctor list for admin/manager filters
            if (userRole == "Admin" || userRole == "Manager")
            {
                try
                {
                    var doctors = await _userService.GetUsersByRoleAsync("Doctor");
                    DoctorSelectList = new SelectList(doctors, "Id", "FullName");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading doctors: {ex.Message}");
                    DoctorSelectList = new SelectList(new List<User>(), "Id", "FullName");
                }
            }
        }

        private async Task LoadStatsAsync()
        {
            try
            {
                var userId = SessionHelper.GetUserId(HttpContext.Session);
                var userRole = SessionHelper.GetUserRole(HttpContext.Session);

                if (!userId.HasValue) return;

                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);

                if (userRole == "Customer")
                {
                    var customerAppointments = await _appointmentService.GetAppointmentsByOwnerAsync(userId.Value);
                    TodayCount = customerAppointments.Count(a => a.AppointmentTime.Date == today);
                    UpcomingCount = customerAppointments.Count(a => a.AppointmentTime.Date > today);
                    PendingCount = customerAppointments.Count(a => a.Status == "Pending");
                    CompletedCount = customerAppointments.Count(a => a.Status == "Completed");
                }
                else if (userRole == "Doctor")
                {
                    var doctorAppointments = await _appointmentService.GetAppointmentsByDoctorAsync(userId.Value, today);
                    TodayCount = doctorAppointments.Count(a => a.AppointmentTime.Date == today);
                    UpcomingCount = doctorAppointments.Count(a => a.AppointmentTime.Date > today);
                    PendingCount = doctorAppointments.Count(a => a.Status == "Pending");
                    CompletedCount = doctorAppointments.Count(a => a.Status == "Completed");
                }
                else
                {
                    var allAppointments = await _appointmentService.GetAllAppointmentsAsync();
                    TodayCount = allAppointments.Count(a => a.AppointmentTime.Date == today);
                    UpcomingCount = allAppointments.Count(a => a.AppointmentTime.Date > today);
                    PendingCount = allAppointments.Count(a => a.Status == "Pending");
                    CompletedCount = allAppointments.Count(a => a.Status == "Completed");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading stats: {ex.Message}");
            }
        }
    }
}