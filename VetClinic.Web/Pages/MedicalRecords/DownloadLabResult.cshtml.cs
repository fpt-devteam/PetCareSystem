using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VetClinic.Service.Interfaces;
using VetClinic.Web.Helpers;

namespace VetClinic.Web.Pages.MedicalRecords
{
    public class DownloadLabResultModel : PageModel
    {
        private readonly ILabResultService _labResultService;
        private readonly IMedicalRecordService _medicalRecordService;

        public DownloadLabResultModel(
            ILabResultService labResultService,
            IMedicalRecordService medicalRecordService)
        {
            _labResultService = labResultService;
            _medicalRecordService = medicalRecordService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            var userId = SessionHelper.GetUserId(HttpContext.Session);
            var userRole = SessionHelper.GetUserRole(HttpContext.Session);

            if (!userId.HasValue)
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                // Get the lab result
                var labResult = await _labResultService.GetLabResultByIdAsync(id);
                if (labResult == null)
                {
                    TempData["ErrorMessage"] = "Lab result not found.";
                    return RedirectToPage("/Pets");
                }

                // Check if user can access the medical record
                var canAccess = await _medicalRecordService.CanUserAccessMedicalRecordAsync(
                    userId.Value, labResult.MedicalRecordId, userRole ?? "Customer");

                if (!canAccess)
                {
                    TempData["ErrorMessage"] = "You don't have permission to access this lab result.";
                    return RedirectToPage("/Pets");
                }

                // Get the file content from database
                var fileContent = await _labResultService.GetLabResultFileAsync(id);
                if (fileContent == null || fileContent.Length == 0)
                {
                    TempData["ErrorMessage"] = "Lab result file not found.";
                    return RedirectToPage("/Pets");
                }

                // Return the file for download
                var fileName = labResult.FileName;
                var contentType = labResult.ContentType ?? "application/octet-stream";

                return File(fileContent, contentType, fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading lab result: {ex.Message}");
                TempData["ErrorMessage"] = "Error downloading lab result.";
                return RedirectToPage("/Pets");
            }
        }
    }
}
