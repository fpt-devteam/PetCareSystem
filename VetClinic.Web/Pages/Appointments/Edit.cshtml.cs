using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VetClinic.Service.Interfaces;
using VetClinic.Repository.Entities;
using VetClinic.Web.Helpers;
using System.ComponentModel.DataAnnotations;
using VetClinic.Repository.Enums;

namespace VetClinic.Web.Pages.Appointments
{
    public class EditModel : PageModel
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IPetService _petService;
        private readonly IUserService _userService;
        private readonly IServiceService _serviceService;

        public EditModel(IAppointmentService appointmentService, IPetService petService, IUserService userService, IServiceService serviceService)
        {
            _appointmentService = appointmentService;
            _petService = petService;
            _userService = userService;
            _serviceService = serviceService;
        }

        [BindProperty]
        public EditAppointmentModel Input { get; set; } = null!;

        public SelectList PetSelectList { get; set; } = null!;
        public SelectList DoctorSelectList { get; set; } = null!;
        public SelectList ServiceSelectList { get; set; } = null!;
        public SelectList StatusSelectList { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _appointmentService.GetAppointmentWithDetailsAsync(id.Value);
            if (appointment == null)
            {
                return NotFound();
            }

            var currentRole = SessionHelper.GetUserRole(HttpContext.Session) ?? "";
            var currentUserId = SessionHelper.GetUserId(HttpContext.Session);

            // Check permissions
            if (currentRole == "Customer")
            {
                // Customer can only edit appointments for their own pets
                if (appointment.Pet?.OwnerId != currentUserId)
                {
                    return Forbid();
                }
            }
            else if (!SessionHelper.IsInAnyRole(HttpContext.Session, "Admin", "Manager", "Staff"))
            {
                return Forbid();
            }

            Input = new EditAppointmentModel
            {
                Id = appointment.Id,
                PetId = appointment.PetId,
                DoctorId = appointment.DoctorId,
                ServiceId = appointment.ServiceId,
                AppointmentTime = appointment.AppointmentTime,
                Status = appointment.Status,
                Notes = appointment.Notes
            };

            await LoadSelectListsAsync(currentRole, currentUserId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var currentRole = SessionHelper.GetUserRole(HttpContext.Session) ?? "";
                var currentUserId = SessionHelper.GetUserId(HttpContext.Session);
                await LoadSelectListsAsync(currentRole, currentUserId);
                return Page();
            }

            var appointment = await _appointmentService.GetAppointmentWithDetailsAsync(Input.Id);
            if (appointment == null)
            {
                return NotFound();
            }

            var userRole = SessionHelper.GetUserRole(HttpContext.Session) ?? "";
            var userId = SessionHelper.GetUserId(HttpContext.Session);

            // Re-check permissions
            if (userRole == "Customer")
            {
                if (appointment.Pet?.OwnerId != userId)
                {
                    return Forbid();
                }

                // Customers can only edit appointment time and notes
                appointment.AppointmentTime = Input.AppointmentTime;
                appointment.Notes = Input.Notes;
            }
            else if (SessionHelper.IsInAnyRole(HttpContext.Session, "Admin", "Manager", "Staff"))
            {
                // Staff can edit all fields
                appointment.PetId = Input.PetId;
                appointment.DoctorId = Input.DoctorId;
                appointment.ServiceId = Input.ServiceId;
                appointment.AppointmentTime = Input.AppointmentTime;
                appointment.Status = Input.Status;
                appointment.Notes = Input.Notes;
            }
            else
            {
                return Forbid();
            }

            try
            {
                await _appointmentService.UpdateAppointmentAsync(appointment);
                TempData["Message"] = "Appointment updated successfully.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error updating appointment: {ex.Message}");
                await LoadSelectListsAsync(userRole, userId);
                return Page();
            }
        }

        private async Task LoadSelectListsAsync(string currentRole, int? currentUserId)
        {
            var pets = currentRole == "Customer" && currentUserId.HasValue
                ? await _petService.GetPetsByOwnerIdAsync(currentUserId.Value)
                : await _petService.GetAllPetsAsync();

            PetSelectList = new SelectList(pets, "Id", "Name");

            var doctors = await _userService.GetUsersByRoleAsync("Doctor");
            DoctorSelectList = new SelectList(doctors, "Id", "FullName");

            var services = await _serviceService.GetAllServicesAsync();
            ServiceSelectList = new SelectList(services, "Id", "Name");

            var statusValues = Enum.GetValues<AppointmentStatus>()
                .Select(e => new { Value = e.ToString(), Text = e.ToString() });
            StatusSelectList = new SelectList(statusValues, "Value", "Text");
        }

        public class EditAppointmentModel
        {
            public int Id { get; set; }

            [Required]
            [Display(Name = "Pet")]
            public int PetId { get; set; }

            [Required]
            [Display(Name = "Doctor")]
            public int DoctorId { get; set; }

            [Required]
            [Display(Name = "Service")]
            public int ServiceId { get; set; }

            [Required]
            [Display(Name = "Appointment Time")]
            public DateTime AppointmentTime { get; set; }

            [Required]
            [Display(Name = "Status")]
            public string Status { get; set; } = AppointmentStatus.Scheduled.ToString();

            [Display(Name = "Notes")]
            public string? Notes { get; set; }
        }
    }
}
