using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using VetClinic.Repository.Entities;
using VetClinic.Service.Interfaces;
using VetClinic.Web.Helpers;

namespace VetClinic.Web.Pages.Appointments
{
    public class BookModel : PageModel
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IPetService _petService;
        private readonly IServiceService _serviceService;
        private readonly IUserService _userService;

        public BookModel(IAppointmentService appointmentService, IPetService petService,
            IServiceService serviceService, IUserService userService)
        {
            _appointmentService = appointmentService;
            _petService = petService;
            _serviceService = serviceService;
            _userService = userService;
        }

        [BindProperty]
        public AppointmentBookingModel Booking { get; set; } = new AppointmentBookingModel();

        public SelectList? PetSelectList { get; set; }
        public SelectList? ServiceSelectList { get; set; }
        public SelectList? DoctorSelectList { get; set; }

        public async Task<IActionResult> OnGetAsync(int? serviceId, int? petId)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            await LoadDataAsync();

            // Pre-select service if provided
            if (serviceId.HasValue)
            {
                Booking.ServiceId = serviceId.Value;
            }

            // Pre-select pet if provided
            if (petId.HasValue)
            {
                Booking.PetId = petId.Value;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            await LoadDataAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var userId = SessionHelper.GetUserId(HttpContext.Session);
                if (!userId.HasValue)
                {
                    ModelState.AddModelError("", "Unable to verify user session.");
                    return Page();
                }

                // Validate that the selected pet belongs to the current user (for customers)
                var userRole = SessionHelper.GetUserRole(HttpContext.Session);
                if (userRole == "Customer")
                {
                    var pet = await _petService.GetPetByIdAsync(Booking.PetId);
                    if (pet == null || pet.OwnerId != userId.Value)
                    {
                        ModelState.AddModelError(nameof(Booking.PetId), "You can only book appointments for your own pets.");
                        return Page();
                    }
                }

                // Book the appointment
                var appointment = await _appointmentService.BookAppointmentAsync(
                    Booking.PetId,
                    Booking.DoctorId,
                    Booking.ServiceId,
                    Booking.AppointmentTime,
                    Booking.Notes
                );

                TempData["SuccessMessage"] = "Appointment booked successfully!";
                return RedirectToPage("/Appointments/Index");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while booking the appointment. Please try again.");
                Console.WriteLine($"Error booking appointment: {ex.Message}");
                return Page();
            }
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var userId = SessionHelper.GetUserId(HttpContext.Session);
                var userRole = SessionHelper.GetUserRole(HttpContext.Session);

                Console.WriteLine($"DEBUG - LoadDataAsync: UserId={userId}, UserRole={userRole}");

                if (userId.HasValue)
                {
                    // Load pets based on user role
                    IEnumerable<Pet> pets;
                    if (userRole == "Customer")
                    {
                        pets = await _petService.GetPetsByOwnerIdAsync(userId.Value);
                        Console.WriteLine($"DEBUG - Customer pets count: {pets.Count()} for ownerId: {userId.Value}");
                    }
                    else
                    {
                        pets = await _petService.GetAllPetsAsync();
                        Console.WriteLine($"DEBUG - All pets count: {pets.Count()}");
                    }
                    PetSelectList = new SelectList(pets, "Id", "Name");

                    // Load active services
                    var services = await _serviceService.GetActiveServicesAsync();
                    Console.WriteLine($"DEBUG - Active services count: {services.Count()}");
                    ServiceSelectList = new SelectList(services, "Id", "Name");

                    // Load doctors
                    var doctors = await _userService.GetUsersByRoleAsync("Doctor");
                    Console.WriteLine($"DEBUG - Doctors count: {doctors.Count()}");
                    DoctorSelectList = new SelectList(doctors, "Id", "FullName");
                }
                else
                {
                    Console.WriteLine("DEBUG - UserId is null!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                PetSelectList = new SelectList(new List<Pet>(), "Id", "Name");
                ServiceSelectList = new SelectList(new List<VetClinic.Repository.Entities.Service>(), "Id", "Name");
                DoctorSelectList = new SelectList(new List<User>(), "Id", "FullName");
            }
        }
    }

    public class AppointmentBookingModel
    {
        [Required(ErrorMessage = "Please select a pet")]
        [Display(Name = "Pet")]
        public int PetId { get; set; }

        [Required(ErrorMessage = "Please select a service")]
        [Display(Name = "Service")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Please select a doctor")]
        [Display(Name = "Doctor")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Please select an appointment date and time")]
        [Display(Name = "Appointment Date & Time")]
        [DataType(DataType.DateTime)]
        public DateTime AppointmentTime { get; set; } = DateTime.Now.AddDays(1).Date.AddHours(9); // Default to 9 AM tomorrow

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        [Display(Name = "Additional Notes")]
        public string? Notes { get; set; }
    }
}