using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using VetClinic.Repository.Entities;
using VetClinic.Service.Interfaces;
using VetClinic.Web.Helpers;

namespace VetClinic.Web.Pages.MedicalRecords
{
    public class IndexModel : PageModel
    {
        private readonly IMedicalRecordService _medicalRecordService;
        private readonly IPetService _petService;

        public IndexModel(IMedicalRecordService medicalRecordService, IPetService petService)
        {
            _medicalRecordService = medicalRecordService;
            _petService = petService;
        }

        public IEnumerable<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? PetId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            // Customers can access medical records for their own pets
            // Doctors, managers, and staff can access all records
            var userRole = SessionHelper.GetUserRole(HttpContext.Session);
            if (!SessionHelper.IsInAnyRole(HttpContext.Session, new[] { "Doctor", "Manager", "Staff", "Customer" }))
            {
                return RedirectToPage("/Index");
            }

            try
            {
                var userId = SessionHelper.GetUserId(HttpContext.Session);
                userRole = SessionHelper.GetUserRole(HttpContext.Session);

                if (PetId.HasValue)
                {
                    // Get medical records for specific pet
                    MedicalRecords = await _medicalRecordService.GetMedicalRecordsByPetAsync(PetId.Value);

                    // For customers, verify they own this pet
                    if (userRole == "Customer" && userId.HasValue)
                    {
                        var pet = await _petService.GetPetByIdAsync(PetId.Value);
                        if (pet == null || pet.OwnerId != userId.Value)
                        {
                            TempData["ErrorMessage"] = "You can only view medical records for your own pets.";
                            return RedirectToPage("/Pets");
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(SearchTerm))
                {
                    // Search medical records based on user role
                    if (userRole == "Customer" && userId.HasValue)
                    {
                        // Customers can only search their own pets' records
                        var customerPets = await _petService.GetPetsByOwnerIdAsync(userId.Value);
                        var petIds = customerPets.Select(p => p.Id).ToList();
                        var allRecords = await _medicalRecordService.GetAllMedicalRecordsAsync();
                        MedicalRecords = allRecords.Where(r =>
                            petIds.Contains(r.PetId) &&
                            ((r.Pet?.Name?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) == true) ||
                             (r.Diagnosis?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) == true) ||
                             (r.TreatmentNotes?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) == true)));
                    }
                    else
                    {
                        // Staff can search all records
                        var allRecords = await _medicalRecordService.GetAllMedicalRecordsAsync();
                        MedicalRecords = allRecords.Where(r =>
                            (r.Pet?.Name?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) == true) ||
                            (r.Diagnosis?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) == true) ||
                            (r.TreatmentNotes?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) == true));
                    }
                }
                else if (userRole == "Doctor" && userId.HasValue)
                {
                    // Show medical records for appointments with this doctor
                    MedicalRecords = await _medicalRecordService.GetMedicalRecordsByDoctorAsync(userId.Value);
                }
                else if (userRole == "Customer" && userId.HasValue)
                {
                    // Customers see medical records for all their pets
                    var customerPets = await _petService.GetPetsByOwnerIdAsync(userId.Value);
                    var petIds = customerPets.Select(p => p.Id).ToList();
                    var allRecords = await _medicalRecordService.GetAllMedicalRecordsAsync();
                    MedicalRecords = allRecords.Where(r => petIds.Contains(r.PetId));
                }
                else
                {
                    // Show all medical records (for managers and staff)
                    MedicalRecords = await _medicalRecordService.GetAllMedicalRecordsAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading medical records: {ex.Message}");
                MedicalRecords = new List<MedicalRecord>();
                TempData["ErrorMessage"] = "Error loading medical records. Please try again.";
            }

            return Page();
        }
    }
}
