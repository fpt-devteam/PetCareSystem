using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

            // Only doctors and managers can access medical records
            if (!SessionHelper.IsInAnyRole(HttpContext.Session, new[] { "Doctor", "Manager" }))
            {
                return RedirectToPage("/Index");
            }

            try
            {
                var userId = SessionHelper.GetUserId(HttpContext.Session);
                var userRole = SessionHelper.GetUserRole(HttpContext.Session);

                if (PetId.HasValue)
                {
                    // Get medical records for specific pet
                    MedicalRecords = await _medicalRecordService.GetMedicalRecordsByPetAsync(PetId.Value);
                }
                else if (!string.IsNullOrEmpty(SearchTerm))
                {
                    // Search medical records - use basic filtering for now
                    var allRecords = await _medicalRecordService.GetAllMedicalRecordsAsync();
                    MedicalRecords = allRecords.Where(r => 
                        (r.Pet?.Name?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) == true) ||
                        (r.Diagnosis?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) == true) ||
                        (r.TreatmentNotes?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) == true));
                }
                else if (userRole == "Doctor" && userId.HasValue)
                {
                    // Show medical records for appointments with this doctor
                    MedicalRecords = await _medicalRecordService.GetMedicalRecordsByDoctorAsync(userId.Value);
                }
                else
                {
                    // Show all medical records (for managers)
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