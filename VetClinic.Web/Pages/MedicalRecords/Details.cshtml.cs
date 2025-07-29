using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VetClinic.Repository.Entities;
using VetClinic.Service.Interfaces;
using VetClinic.Web.Helpers;

namespace VetClinic.Web.Pages.MedicalRecords
{
    public class DetailsModel : PageModel
    {
        private readonly IMedicalRecordService _medicalRecordService;
        private readonly IPetService _petService;
        private readonly ILabResultService _labResultService;

        public DetailsModel(
            IMedicalRecordService medicalRecordService,
            IPetService petService,
            ILabResultService labResultService)
        {
            _medicalRecordService = medicalRecordService;
            _petService = petService;
            _labResultService = labResultService;
        }

        public MedicalRecord? MedicalRecord { get; set; }
        public IEnumerable<LabResult> LabResults { get; set; } = new List<LabResult>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                // Get the medical record with all related data
                MedicalRecord = await _medicalRecordService.GetMedicalRecordByIdAsync(id);

                if (MedicalRecord == null)
                {
                    TempData["ErrorMessage"] = "Medical record not found.";
                    return RedirectToPage("/Pets");
                }

                // Check authorization
                var userId = SessionHelper.GetUserId(HttpContext.Session);
                var userRole = SessionHelper.GetUserRole(HttpContext.Session);

                if (!userId.HasValue)
                {
                    return RedirectToPage("/Account/Login");
                }

                // Check if user can access this medical record
                var canAccess = await _medicalRecordService.CanUserAccessMedicalRecordAsync(
                    userId.Value, id, userRole ?? "Customer");

                if (!canAccess)
                {
                    TempData["ErrorMessage"] = "You don't have permission to view this medical record.";
                    return RedirectToPage("/Pets");
                }

                // Load lab results for this medical record
                LabResults = await _labResultService.GetLabResultsByMedicalRecordAsync(id);

                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading medical record details: {ex.Message}");
                TempData["ErrorMessage"] = "Error loading medical record information.";
                return RedirectToPage("/Pets");
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
