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
            if (!ViewSessionHelper.IsInAnyRole(HttpContext.Session, "Admin", "Manager"))
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

        public async Task<IActionResult> OnPostDeactivateAsync(int userId)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            // Only admins can deactivate users
            if (!ViewSessionHelper.IsInRole(HttpContext.Session, "Admin"))
            {
                TempData["ErrorMessage"] = "You are not authorized to deactivate users.";
                return RedirectToPage();
            }

            var currentUserId = ViewSessionHelper.GetUserId(HttpContext.Session);
            if (currentUserId == userId)
            {
                TempData["ErrorMessage"] = "You cannot deactivate your own account.";
                return RedirectToPage();
            }

            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                if (user != null)
                {
                    user.IsActive = false;
                    var updatedUser = await _userService.UpdateUserAsync(user);
                    if (updatedUser != null)
                    {
                        TempData["SuccessMessage"] = "User has been deactivated successfully.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "User could not be deactivated.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "User not found.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deactivating user: {ex.Message}");
                TempData["ErrorMessage"] = "Error deactivating user. Please try again.";
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
