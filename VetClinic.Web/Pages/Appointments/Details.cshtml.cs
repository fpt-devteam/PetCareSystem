using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VetClinic.Repository.Entities;
using VetClinic.Service.Interfaces;
using VetClinic.Web.Helpers;
using VetClinic.Web.Services;

namespace VetClinic.Web.Pages.Appointments
{
    public class DetailsModel : PageModel
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IAppointmentNotificationService _notificationService;

        public DetailsModel(IAppointmentService appointmentService, IAppointmentNotificationService notificationService)
        {
            _appointmentService = appointmentService;
            _notificationService = notificationService;
        }

        public Appointment Appointment { get; set; } = new();
        public bool CanEdit { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Get appointment with all related data
            var appointment = await _appointmentService.GetAppointmentWithDetailsAsync(id);
            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Appointment not found.";
                return RedirectToPage("./Index");
            }

            Appointment = appointment;

            // Check permissions
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");
            
            // Admin, Manager, Staff can view all appointments
            if (ViewSessionHelper.IsInAnyRole(HttpContext.Session, "Admin", "Manager", "Staff", "Doctor"))
            {
                CanEdit = true;
            }
            // Customer can only view their own pet's appointments
            else if (ViewSessionHelper.IsInRole(HttpContext.Session, "Customer"))
            {
                if (appointment.Pet?.OwnerId != currentUserId)
                {
                    TempData["ErrorMessage"] = "You can only view appointments for your own pets.";
                    return RedirectToPage("./Index");
                }
                
                // Customer can edit if appointment is not completed/cancelled and is upcoming
                CanEdit = appointment.Status != "Completed" && 
                         appointment.Status != "Cancelled" && 
                         appointment.AppointmentTime > DateTime.Now;
            }
            else
            {
                TempData["ErrorMessage"] = "Access denied.";
                return RedirectToPage("/Account/Login");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int appointmentId, string newStatus)
        {
            try
            {
                // Check permissions - staff and doctors can update status
                if (!ViewSessionHelper.IsInAnyRole(HttpContext.Session, "Admin", "Manager", "Staff", "Doctor"))
                {
                    TempData["ErrorMessage"] = "You don't have permission to update appointment status.";
                    return RedirectToPage("./Details", new { id = appointmentId });
                }

                var currentUserId = HttpContext.Session.GetInt32("UserId") ?? 0;
                var userRole = HttpContext.Session.GetString("UserRole");
                
                // Get appointment to check permissions and get pet owner info
                var appointment = await _appointmentService.GetAppointmentWithDetailsAsync(appointmentId);
                if (appointment == null)
                {
                    TempData["ErrorMessage"] = "Appointment not found.";
                    return RedirectToPage("./Index");
                }

                // Additional check: Doctor can only update their own appointments
                if (userRole == "Doctor" && appointment.DoctorId != currentUserId)
                {
                    TempData["ErrorMessage"] = "You can only update status for your own appointments.";
                    return RedirectToPage("./Details", new { id = appointmentId });
                }

                var success = await _appointmentService.UpdateAppointmentStatusAsync(appointmentId, newStatus, currentUserId);
                if (success)
                {
                    // Send real-time notification
                    await _notificationService.NotifyAppointmentStatusChanged(appointmentId, newStatus, appointment.Pet?.OwnerId ?? 0);
                    
                    TempData["SuccessMessage"] = $"Appointment status updated to {newStatus}.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update appointment status.";
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the appointment status.";
            }

            return RedirectToPage("./Details", new { id = appointmentId });
        }

        public async Task<IActionResult> OnPostCancelAsync(int appointmentId)
        {
            try
            {
                var currentUserId = HttpContext.Session.GetInt32("UserId") ?? 0;
                
                // Get appointment to check permissions
                var appointment = await _appointmentService.GetAppointmentWithDetailsAsync(appointmentId);
                if (appointment == null)
                {
                    TempData["ErrorMessage"] = "Appointment not found.";
                    return RedirectToPage("./Index");
                }

                // Check permissions
                bool canCancel = false;
                
                if (ViewSessionHelper.IsInAnyRole(HttpContext.Session, "Admin", "Manager"))
                {
                    // Admin/Manager can cancel any non-completed appointment
                    canCancel = appointment.Status != "Completed";
                }
                else if (ViewSessionHelper.IsInRole(HttpContext.Session, "Customer"))
                {
                    // Customer can only cancel their own upcoming appointments (24+ hours ahead)
                    canCancel = appointment.Pet?.OwnerId == currentUserId &&
                               appointment.Status == "Scheduled" &&
                               appointment.AppointmentTime > DateTime.Now.AddHours(24);
                }

                if (!canCancel)
                {
                    TempData["ErrorMessage"] = "You cannot cancel this appointment.";
                    return RedirectToPage("./Details", new { id = appointmentId });
                }

                var success = await _appointmentService.CancelAppointmentAsync(appointmentId, currentUserId, "Cancelled from Details page");
                if (success)
                {
                    // Send real-time notification
                    await _notificationService.NotifyAppointmentCancelled(
                        appointmentId, 
                        appointment.Pet?.OwnerId ?? 0, 
                        appointment.Pet?.Name ?? "Unknown Pet", 
                        appointment.AppointmentTime);
                    
                    TempData["SuccessMessage"] = "Appointment has been cancelled successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to cancel appointment.";
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while cancelling the appointment.";
            }

            return RedirectToPage("./Details", new { id = appointmentId });
        }
    }
}
