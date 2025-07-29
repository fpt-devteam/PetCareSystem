using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VetClinic.Service.Interfaces;
using VetClinic.Web.Helpers;

namespace VetClinic.Web.Pages.MedicalRecords
{
    public class DeleteLabResultModel : PageModel
    {
        private readonly ILabResultService _labResultService;

        public DeleteLabResultModel(ILabResultService labResultService)
        {
            _labResultService = labResultService;
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return new JsonResult(new { success = false, message = "Not authenticated" });
            }

            var userId = SessionHelper.GetUserId(HttpContext.Session);
            var userRole = SessionHelper.GetUserRole(HttpContext.Session);

            if (!userId.HasValue || userRole != "Doctor")
            {
                return new JsonResult(new { success = false, message = "Unauthorized" });
            }

            try
            {
                // Get the lab result to check ownership
                var labResult = await _labResultService.GetLabResultByIdAsync(id);
                if (labResult == null)
                {
                    return new JsonResult(new { success = false, message = "Lab result not found" });
                }

                // Check if the user uploaded this lab result
                if (labResult.UploadedBy != userId.Value)
                {
                    return new JsonResult(new { success = false, message = "You can only delete lab results that you uploaded" });
                }

                // Delete the lab result
                await _labResultService.DeleteLabResultAsync(id);

                return new JsonResult(new { success = true, message = "Lab result deleted successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting lab result: {ex.Message}");
                return new JsonResult(new { success = false, message = "Error deleting lab result. Please try again." });
            }
        }
    }
}
