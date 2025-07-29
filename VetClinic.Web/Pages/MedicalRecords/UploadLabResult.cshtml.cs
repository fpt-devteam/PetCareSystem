using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VetClinic.Repository.Entities;
using VetClinic.Service.Interfaces;
using VetClinic.Web.Helpers;
using Microsoft.AspNetCore.Http;

namespace VetClinic.Web.Pages.MedicalRecords
{
    [RequestSizeLimit(20 * 1024 * 1024)] // 20MB limit
    public class UploadLabResultModel : PageModel
    {
        private readonly ILabResultService _labResultService;
        private readonly IMedicalRecordService _medicalRecordService;

        [BindProperty]
        public IFormFile? UploadedFile { get; set; }

        [BindProperty]
        public string LabName { get; set; } = string.Empty;

        [BindProperty]
        public string Description { get; set; } = string.Empty;

        [BindProperty]
        public DateTime? TestDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public int MedicalRecordId { get; set; }

        public MedicalRecord? MedicalRecord { get; set; }

        public UploadLabResultModel(
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

            if (!userId.HasValue || userRole != "Doctor")
            {
                TempData["ErrorMessage"] = "You are not authorized to upload lab results.";
                return RedirectToPage("/MedicalRecords/Details", new { id });
            }

            // Check if medical record exists and user has access
            var canAccess = await _medicalRecordService.CanUserAccessMedicalRecordAsync(
                userId.Value, id, userRole);

            if (!canAccess)
            {
                TempData["ErrorMessage"] = "You don't have access to this medical record.";
                return RedirectToPage("/Pets");
            }

            MedicalRecordId = id;
            MedicalRecord = await _medicalRecordService.GetMedicalRecordByIdAsync(id);

            if (MedicalRecord == null)
            {
                TempData["ErrorMessage"] = "Medical record not found.";
                return RedirectToPage("/Pets");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            // Set the MedicalRecordId from the route parameter
            MedicalRecordId = id;
            Console.WriteLine($"OnPostAsync called. MedicalRecordId: {MedicalRecordId}");

            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            var userId = SessionHelper.GetUserId(HttpContext.Session);
            var userRole = SessionHelper.GetUserRole(HttpContext.Session);

            if (!userId.HasValue || userRole != "Doctor")
            {
                TempData["ErrorMessage"] = "You are not authorized to upload lab results.";
                return RedirectToPage("/MedicalRecords/Details", new { id = MedicalRecordId });
            }

            // Check if medical record exists and user has access
            var canAccess = await _medicalRecordService.CanUserAccessMedicalRecordAsync(
                userId.Value, MedicalRecordId, userRole);

            if (!canAccess)
            {
                TempData["ErrorMessage"] = "You don't have access to this medical record.";
                return RedirectToPage("/Pets");
            }

            // Validate file
            if (UploadedFile == null || UploadedFile.Length == 0)
            {
                Console.WriteLine("File validation failed: No file uploaded");
                ModelState.AddModelError("UploadedFile", "Please select a file to upload.");
                MedicalRecord = await _medicalRecordService.GetMedicalRecordByIdAsync(MedicalRecordId);
                return Page();
            }

            // Validate file size (10MB)
            if (UploadedFile.Length > 10 * 1024 * 1024)
            {
                Console.WriteLine($"File validation failed: File too large ({UploadedFile.Length} bytes)");
                ModelState.AddModelError("UploadedFile", "File size exceeds 10MB limit.");
                MedicalRecord = await _medicalRecordService.GetMedicalRecordByIdAsync(MedicalRecordId);
                return Page();
            }

            // Validate file type
            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(UploadedFile.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                Console.WriteLine($"File validation failed: Invalid file type ({fileExtension})");
                ModelState.AddModelError("UploadedFile", "Invalid file type. Only PDF, JPG, JPEG, PNG, and GIF files are allowed.");
                MedicalRecord = await _medicalRecordService.GetMedicalRecordByIdAsync(MedicalRecordId);
                return Page();
            }

            try
            {
                // Read file content
                byte[] fileContent;
                using (var ms = new MemoryStream())
                {
                    await UploadedFile.CopyToAsync(ms);
                    fileContent = ms.ToArray();
                }

                Console.WriteLine($"File content size: {fileContent.Length} bytes");

                // Save to database
                await _labResultService.UploadLabResultAsync(
                    medicalRecordId: MedicalRecordId,
                    uploadedBy: userId.Value,
                    fileName: UploadedFile.FileName,
                    fileType: fileExtension,
                    fileSize: UploadedFile.Length,
                    contentType: UploadedFile.ContentType ?? "application/octet-stream",
                    fileContent: fileContent,
                    description: Description,
                    labName: LabName,
                    testDate: TestDate);

                TempData["SuccessMessage"] = "Lab result uploaded successfully.";
                return RedirectToPage("/MedicalRecords/Details", new { id = MedicalRecordId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading lab result: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                ModelState.AddModelError(string.Empty, $"Error uploading file: {ex.Message}");
                MedicalRecord = await _medicalRecordService.GetMedicalRecordByIdAsync(MedicalRecordId);
                return Page();
            }
        }
    }
}
