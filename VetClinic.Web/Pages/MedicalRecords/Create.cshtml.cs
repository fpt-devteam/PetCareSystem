using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using VetClinic.Repository.Entities;
using VetClinic.Service.Interfaces;
using VetClinic.Web.Helpers;

namespace VetClinic.Web.Pages.MedicalRecords
{
    public class CreateModel : PageModel
    {
        private readonly IMedicalRecordService _medicalRecordService;
        private readonly IAppointmentService _appointmentService;
        private readonly IPetService _petService;
        private readonly IUserService _userService;

        public CreateModel(
            IMedicalRecordService medicalRecordService,
            IAppointmentService appointmentService,
            IPetService petService,
            IUserService userService)
        {
            _medicalRecordService = medicalRecordService;
            _appointmentService = appointmentService;
            _petService = petService;
            _userService = userService;
        }

        [BindProperty]
        public MedicalRecordCreateModel MedicalRecord { get; set; } = new MedicalRecordCreateModel();

        public SelectList? AppointmentSelectList { get; set; }
        public SelectList? PetSelectList { get; set; }
        public SelectList? DoctorSelectList { get; set; }

        // For pre-selecting based on URL parameters
        public int? PreSelectedAppointmentId { get; set; }
        public int? PreSelectedPetId { get; set; }

        public async Task<IActionResult> OnGetAsync(int? appointmentId, int? petId)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            // Only doctors and managers can create medical records
            if (!SessionHelper.IsInAnyRole(HttpContext.Session, new[] { "Doctor", "Manager" }))
            {
                return RedirectToPage("/Index");
            }

            PreSelectedAppointmentId = appointmentId;
            PreSelectedPetId = petId;

            await LoadDataAsync();

            // Pre-populate fields if appointment is specified
            if (appointmentId.HasValue)
            {
                await PrePopulateFromAppointmentAsync(appointmentId.Value);
            }
            else if (petId.HasValue)
            {
                MedicalRecord.PetId = petId.Value;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            // Only doctors and managers can create medical records
            if (!SessionHelper.IsInAnyRole(HttpContext.Session, new[] { "Doctor", "Manager" }))
            {
                return RedirectToPage("/Index");
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

                var userRole = SessionHelper.GetUserRole(HttpContext.Session);

                // Create medical record entity
                var medicalRecord = new MedicalRecord
                {
                    AppointmentId = MedicalRecord.AppointmentId,
                    PetId = MedicalRecord.PetId,
                    DoctorId = MedicalRecord.DoctorId,
                    VisitDate = MedicalRecord.VisitDate,
                    Diagnosis = MedicalRecord.Diagnosis,
                    TreatmentNotes = MedicalRecord.TreatmentNotes,
                    Prescription = MedicalRecord.Prescription,
                    CreatedDate = DateTime.UtcNow
                };

                // Validate doctor assignment for regular doctors
                if (userRole == "Doctor" && medicalRecord.DoctorId != userId.Value)
                {
                    ModelState.AddModelError(nameof(MedicalRecord.DoctorId), "You can only create medical records assigned to yourself.");
                    return Page();
                }

                await _medicalRecordService.CreateMedicalRecordAsync(medicalRecord);
                TempData["SuccessMessage"] = "Medical record has been created successfully.";
                return RedirectToPage("./Index");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating medical record: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while creating the medical record. Please try again.");
                return Page();
            }
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var userId = SessionHelper.GetUserId(HttpContext.Session);
                var userRole = SessionHelper.GetUserRole(HttpContext.Session);

                // Load appointments (completed ones primarily)
                var allAppointments = await _appointmentService.GetAllAppointmentsAsync();
                var availableAppointments = allAppointments.Where(a => a.Status == "Completed" || a.Status == "Scheduled");
                AppointmentSelectList = new SelectList(availableAppointments, "Id", "AppointmentTime");

                // Load all pets
                var pets = await _petService.GetAllPetsAsync();
                PetSelectList = new SelectList(pets, "Id", "Name");

                // Load doctors
                var doctors = await _userService.GetUsersByRoleAsync("Doctor");

                // If user is a doctor, default to themselves
                if (userRole == "Doctor" && userId.HasValue)
                {
                    MedicalRecord.DoctorId = userId.Value;
                    DoctorSelectList = new SelectList(doctors, "Id", "FullName", userId.Value);
                }
                else
                {
                    DoctorSelectList = new SelectList(doctors, "Id", "FullName");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
                AppointmentSelectList = new SelectList(new List<Appointment>(), "Id", "AppointmentTime");
                PetSelectList = new SelectList(new List<Pet>(), "Id", "Name");
                DoctorSelectList = new SelectList(new List<User>(), "Id", "FullName");
            }
        }

        private async Task PrePopulateFromAppointmentAsync(int appointmentId)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentWithDetailsAsync(appointmentId);
                if (appointment != null)
                {
                    MedicalRecord.AppointmentId = appointment.Id;
                    MedicalRecord.PetId = appointment.PetId;
                    MedicalRecord.DoctorId = appointment.DoctorId;
                    MedicalRecord.VisitDate = appointment.AppointmentTime;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error pre-populating from appointment: {ex.Message}");
            }
        }
    }

    public class MedicalRecordCreateModel
    {
        [Required(ErrorMessage = "Please select an appointment")]
        [Display(Name = "Appointment")]
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Please select a pet")]
        [Display(Name = "Pet")]
        public int PetId { get; set; }

        [Required(ErrorMessage = "Please select a doctor")]
        [Display(Name = "Doctor")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Visit date is required")]
        [Display(Name = "Visit Date")]
        [DataType(DataType.DateTime)]
        public DateTime VisitDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Diagnosis is required")]
        [StringLength(500, ErrorMessage = "Diagnosis cannot exceed 500 characters")]
        [Display(Name = "Diagnosis")]
        public string Diagnosis { get; set; } = string.Empty;

        [Required(ErrorMessage = "Treatment notes are required")]
        [StringLength(2000, ErrorMessage = "Treatment notes cannot exceed 2000 characters")]
        [Display(Name = "Treatment Notes")]
        public string TreatmentNotes { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Prescription cannot exceed 1000 characters")]
        [Display(Name = "Prescription")]
        public string? Prescription { get; set; }
    }
}