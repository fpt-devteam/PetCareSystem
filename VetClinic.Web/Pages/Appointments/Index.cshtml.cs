using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VetClinic.Repository.Entities;
using VetClinic.Service.Interfaces;
using VetClinic.Web.Helpers;
using VetClinic.Web.Services;

namespace VetClinic.Web.Pages.Appointments
{
    public class IndexModel : PageModel
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IUserService _userService;
        private readonly IAppointmentNotificationService _notificationService;

        public IndexModel(
            IAppointmentService appointmentService, 
            IUserService userService,
            IAppointmentNotificationService notificationService)
        {
            _appointmentService = appointmentService;
            _userService = userService;
            _notificationService = notificationService;
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

        [BindProperty(SupportsGet = true)]
        public bool HideCancelled { get; set; } = true; // Default to hide cancelled appointments

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

            // Add no-cache headers
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

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

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("=== OnPostAsync called (default handler) ===");
            Console.WriteLine($"All form data: {string.Join(", ", Request.Form.Select(f => $"{f.Key}={f.Value}"))}");
            
            return await OnGetAsync();
        }

        public async Task<IActionResult> OnPostCancelAsync(int appointmentId)
        {
            Console.WriteLine($"OnPostCancelAsync called with appointmentId: {appointmentId}");
            Console.WriteLine($"X-Requested-With header: {Request.Headers["X-Requested-With"]}");
            Console.WriteLine($"All headers: {string.Join(", ", Request.Headers.Select(h => $"{h.Key}={h.Value}"))}");
            
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                var userId = SessionHelper.GetUserId(HttpContext.Session);
                var userRole = SessionHelper.GetUserRole(HttpContext.Session) ?? "";
                
                if (!userId.HasValue)
                {
                    TempData["ErrorMessage"] = "Unable to verify user session.";
                    return RedirectToPage();
                }

                var result = await _appointmentService.CancelAppointmentAsync(appointmentId, userId.Value, userRole);
                
                if (result)
                {
                    TempData["SuccessMessage"] = "Appointment has been cancelled successfully.";
                    
                    // Get appointment details for SignalR notification
                    var appointment = await _appointmentService.GetAppointmentWithDetailsAsync(appointmentId);
                    if (appointment != null && appointment.Pet != null)
                    {
                        // Send SignalR notification
                        await _notificationService.NotifyAppointmentCancelled(
                            appointmentId, 
                            appointment.Pet.OwnerId, 
                            appointment.Pet.Name, 
                            appointment.AppointmentTime);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to cancel appointment. It may not exist, be in the past, or you may not have permission.";
                }
            }
            catch (UnauthorizedAccessException)
            {
                TempData["ErrorMessage"] = "You are not authorized to cancel this appointment.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error cancelling appointment: {ex.Message}";
            }

            // Check if this is an AJAX request by checking headers
            bool isAjaxRequest = Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                                Request.Headers.ContainsKey("X-Requested-With");

            Console.WriteLine($"Is AJAX request: {isAjaxRequest}");
            Console.WriteLine($"Success message: {TempData["SuccessMessage"]}");
            Console.WriteLine($"Error message: {TempData["ErrorMessage"]}");

            if (isAjaxRequest)
            {
                Console.WriteLine("Returning JSON response");
                // Return JSON response for AJAX requests
                if (TempData["SuccessMessage"] != null)
                {
                    var successMessage = TempData["SuccessMessage"]?.ToString() ?? "Operation completed successfully";
                    return new JsonResult(new { success = true, message = successMessage });
                }
                else
                {
                    var errorMessage = TempData["ErrorMessage"]?.ToString() ?? "Unknown error occurred";
                    return new JsonResult(new { success = false, message = errorMessage });
                }
            }

            Console.WriteLine("Returning page redirect");
            return RedirectToPage();
        }

        private async Task LoadDataAsync()
        {
            var userId = SessionHelper.GetUserId(HttpContext.Session);
            var userRole = SessionHelper.GetUserRole(HttpContext.Session);

            Console.WriteLine($"LoadDataAsync: UserId={userId}, UserRole={userRole}");

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
                Console.WriteLine($"Customer appointments loaded: {Appointments.Count()} total");
            }
            else if (userRole == "Doctor")
            {
                // Doctors see only confirmed appointments and those with advanced status (not scheduled/pending)
                var doctorAppointments = await _appointmentService.GetAllAppointmentsByDoctorAsync(userId.Value);
                // Filter to show only confirmed appointments or those in progress/completed
                var allowedStatuses = new[] { "Confirmed", "InProgress", "Completed", "NoShow" };
                Appointments = doctorAppointments.Where(a => allowedStatuses.Contains(a.Status));
                Console.WriteLine($"Doctor appointments loaded: {Appointments.Count()} total (filtered for confirmed+ status)");
            }
            else
            {
                // Admin, Manager, Staff see all appointments
                var allAppointments = await _appointmentService.GetAllAppointmentsAsync();
                Appointments = allAppointments;
                Console.WriteLine($"All appointments loaded: {Appointments.Count()} total");
            }

            // Log status of each appointment for debugging
            Console.WriteLine("Appointment statuses before filtering:");
            foreach (var apt in Appointments.Take(5)) // Log first 5 for debugging
            {
                Console.WriteLine($"  - Appointment {apt.Id}: Status = {apt.Status}, Time = {apt.AppointmentTime}");
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

            // Hide cancelled appointments by default (unless specifically filtering for them)
            if (HideCancelled && StatusFilter != "Cancelled")
            {
                Appointments = Appointments.Where(a => a.Status != "Cancelled");
                Console.WriteLine($"After hiding cancelled: {Appointments.Count()} appointments");
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
                    var doctorAppointments = await _appointmentService.GetAllAppointmentsByDoctorAsync(userId.Value);
                    // Filter for confirmed and advanced status appointments only
                    var allowedStatuses = new[] { "Confirmed", "InProgress", "Completed", "NoShow" };
                    var filteredAppointments = doctorAppointments.Where(a => allowedStatuses.Contains(a.Status));
                    
                    TodayCount = filteredAppointments.Count(a => a.AppointmentTime.Date == today);
                    UpcomingCount = filteredAppointments.Count(a => a.AppointmentTime.Date > today);
                    PendingCount = filteredAppointments.Count(a => a.Status == "Confirmed"); // Confirmed appointments waiting to start
                    CompletedCount = filteredAppointments.Count(a => a.Status == "Completed" && a.AppointmentTime.Date == today);
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