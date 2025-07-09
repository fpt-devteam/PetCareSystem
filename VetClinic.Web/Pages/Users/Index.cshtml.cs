using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VetClinic.Service.Interfaces;
using VetClinic.Repository.Entities;
using VetClinic.Web.Helpers;

namespace VetClinic.Web.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;

        public IndexModel(IUserService userService)
        {
            _userService = userService;
        }

        public IEnumerable<User> Users { get; set; } = new List<User>();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? RoleFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            // Only admins and managers can view users list
            if (!SessionHelper.IsInAnyRole(HttpContext.Session, new[] { "Admin", "Manager" }))
            {
                return RedirectToPage("/Index");
            }

            try
            {
                var allUsers = await _userService.GetAllUsersAsync();
                Users = FilterUsers(allUsers);
                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading users: {ex.Message}");
                TempData["ErrorMessage"] = "Error loading users. Please try again.";
                Users = new List<User>();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int userId)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            // Only admins can delete users
            if (!SessionHelper.IsInRole(HttpContext.Session, "Admin"))
            {
                TempData["ErrorMessage"] = "You are not authorized to delete users.";
                return RedirectToPage();
            }

            var currentUserId = SessionHelper.GetUserId(HttpContext.Session);
            if (currentUserId == userId)
            {
                TempData["ErrorMessage"] = "You cannot delete your own account.";
                return RedirectToPage();
            }

            try
            {
                var success = await _userService.DeleteUserAsync(userId);
                if (success)
                {
                    TempData["SuccessMessage"] = "User has been deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "User not found or could not be deleted.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user: {ex.Message}");
                TempData["ErrorMessage"] = "Error deleting user. Please try again.";
            }

            return RedirectToPage();
        }

        private IEnumerable<User> FilterUsers(IEnumerable<User> users)
        {
            // Apply search filter
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                var searchLower = SearchTerm.ToLower();
                users = users.Where(u => 
                    u.FullName.ToLower().Contains(searchLower) || 
                    u.Email.ToLower().Contains(searchLower));
            }

            // Apply role filter
            if (!string.IsNullOrEmpty(RoleFilter))
            {
                users = users.Where(u => u.Role == RoleFilter);
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(StatusFilter))
            {
                var isActive = bool.Parse(StatusFilter);
                users = users.Where(u => u.IsActive == isActive);
            }

            return users.OrderBy(u => u.FullName);
        }
    }
}
