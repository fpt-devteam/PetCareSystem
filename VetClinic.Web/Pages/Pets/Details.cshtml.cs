using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VetClinic.Repository.Entities;
using VetClinic.Service.Interfaces;
using VetClinic.Web.Helpers;

namespace VetClinic.Web.Pages.Pets
{
    public class DetailsModel : PageModel
    {
        private readonly IPetService _petService;
        private readonly IAppointmentService _appointmentService;
        private readonly IMedicalRecordService _medicalRecordService;

        public DetailsModel(
            IPetService petService,
            IAppointmentService appointmentService,
            IMedicalRecordService medicalRecordService)
        {
            _petService = petService;
            _appointmentService = appointmentService;
            _medicalRecordService = medicalRecordService;
        }

        public Pet? Pet { get; set; }
        public IEnumerable<Appointment> Appointments { get; set; } = new List<Appointment>();
        public IEnumerable<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int UpcomingAppointments { get; set; }
        public decimal TotalSpent { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                // Get the pet with owner details
                Pet = await _petService.GetPetWithOwnerAsync(id);
                if (Pet == null)
                {
                    TempData["ErrorMessage"] = "Pet not found.";
                    return RedirectToPage("./Index");
                }

                // Check authorization
                var userId = SessionHelper.GetUserId(HttpContext.Session);
                var userRole = SessionHelper.GetUserRole(HttpContext.Session);

                if (!userId.HasValue)
                {
                    return RedirectToPage("/Account/Login");
                }

                // Customers can only view their own pets
                if (userRole == "Customer" && Pet.OwnerId != userId.Value)
                {
                    TempData["ErrorMessage"] = "You can only view your own pets.";
                    return RedirectToPage("./Index");
                }

                // Load related data
                await LoadPetDataAsync(id);

                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading pet details: {ex.Message}");
                TempData["ErrorMessage"] = "Error loading pet information.";
                return RedirectToPage("./Index");
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int petId)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                // Get the pet to check authorization
                var pet = await _petService.GetPetByIdAsync(petId);
                if (pet == null)
                {
                    TempData["ErrorMessage"] = "Pet not found.";
                    return RedirectToPage("./Index");
                }

                var userId = SessionHelper.GetUserId(HttpContext.Session);
                var userRole = SessionHelper.GetUserRole(HttpContext.Session);

                if (!userId.HasValue)
                {
                    return RedirectToPage("/Account/Login");
                }

                // Authorization check
                if (userRole == "Customer" && pet.OwnerId != userId.Value)
                {
                    TempData["ErrorMessage"] = "You can only delete your own pets.";
                    return RedirectToPage("./Index");
                }

                // Check if pet has appointments
                var appointments = await _appointmentService.GetAllAppointmentsAsync();
                var petAppointments = appointments.Where(a => a.PetId == petId);

                if (petAppointments.Any())
                {
                    TempData["ErrorMessage"] = "Cannot delete pet with existing appointments. Please cancel all appointments first.";
                    return RedirectToPage(new { id = petId });
                }

                // Delete the pet
                var success = await _petService.DeletePetAsync(petId);
                if (success)
                {
                    TempData["SuccessMessage"] = $"Pet '{pet.Name}' has been deleted successfully.";
                    return RedirectToPage("./Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete pet. Please try again.";
                    return RedirectToPage(new { id = petId });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting pet: {ex.Message}");
                TempData["ErrorMessage"] = "Error deleting pet. Please try again.";
                return RedirectToPage("./Index");
            }
        }

        private async Task LoadPetDataAsync(int petId)
        {
            try
            {
                // Load appointments for this pet
                var allAppointments = await _appointmentService.GetAllAppointmentsAsync();
                Appointments = allAppointments.Where(a => a.PetId == petId)
                    .OrderByDescending(a => a.AppointmentTime)
                    .Take(10); // Show last 10 appointments

                // Load medical records for this pet
                MedicalRecords = await _medicalRecordService.GetMedicalRecordsByPetAsync(petId);

                // Calculate statistics
                TotalAppointments = allAppointments.Count(a => a.PetId == petId);
                CompletedAppointments = allAppointments.Count(a => a.PetId == petId && a.Status == "Completed");
                UpcomingAppointments = allAppointments.Count(a => a.PetId == petId &&
                    a.AppointmentTime > DateTime.Now && a.Status == "Scheduled");

                // Calculate total spent (completed appointments)
                TotalSpent = allAppointments
                    .Where(a => a.PetId == petId && a.Status == "Completed" && a.Service != null)
                    .Sum(a => a.Service.Price);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading pet data: {ex.Message}");
                // Set safe defaults
                Appointments = new List<Appointment>();
                MedicalRecords = new List<MedicalRecord>();
                TotalAppointments = CompletedAppointments = UpcomingAppointments = 0;
                TotalSpent = 0m;
            }
        }

        public int CalculateAge(DateTime? birthDate)
        {
            if (!birthDate.HasValue) return 0;

            var today = DateTime.Today;
            var age = today.Year - birthDate.Value.Year;
            if (birthDate.Value.Date > today.AddYears(-age)) age--;
            return age;
        }

        public string GetAgeString(DateTime? birthDate)
        {
            if (!birthDate.HasValue) return "Unknown";

            var age = CalculateAge(birthDate);
            if (age == 0)
            {
                var months = (DateTime.Today.Year - birthDate.Value.Year) * 12 + DateTime.Today.Month - birthDate.Value.Month;
                if (months <= 0) months = 1;
                return $"{months} month{(months == 1 ? "" : "s")}";
            }
            return $"{age} year{(age == 1 ? "" : "s")}";
        }
    }
}