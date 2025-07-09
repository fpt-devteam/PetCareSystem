using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VetClinic.Web.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Clear all session data
            HttpContext.Session.Clear();
            
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToPage("/Index");
        }

        public IActionResult OnPost()
        {
            return OnGet();
        }
    }
}
