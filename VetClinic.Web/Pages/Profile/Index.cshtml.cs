using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using VetClinic.Repository.Entities;
using VetClinic.Service.Interfaces;
using VetClinic.Web.Helpers;

namespace VetClinic.Web.Pages.Profile
{
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;

        public IndexModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public UserProfileModel UserProfile { get; set; } = new UserProfileModel();

        public async Task<IActionResult> OnGetAsync()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            var userId = SessionHelper.GetUserId(HttpContext.Session);
            if (!userId.HasValue)
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                var user = await _userService.GetUserByIdAsync(userId.Value);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToPage("/Account/Login");
                }

                UserProfile = new UserProfileModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    Role = user.Role.ToString()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading user profile: {ex.Message}");
                TempData["ErrorMessage"] = "Error loading profile. Please try again.";
                return RedirectToPage("/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            var userId = SessionHelper.GetUserId(HttpContext.Session);
            if (!userId.HasValue)
            {
                return RedirectToPage("/Account/Login");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var currentUser = await _userService.GetUserByIdAsync(userId.Value);
                if (currentUser == null)
                {
                    ModelState.AddModelError("", "User not found.");
                    return Page();
                }

                // Check if email is being changed and if it's already taken
                if (currentUser.Email != UserProfile.Email)
                {
                    var existingUser = await _userService.GetUserByEmailAsync(UserProfile.Email);
                    if (existingUser != null)
                    {
                        ModelState.AddModelError(nameof(UserProfile.Email), "This email is already in use.");
                        return Page();
                    }
                }

                // Update user information
                currentUser.Email = UserProfile.Email;
                currentUser.FullName = UserProfile.FullName;
                currentUser.PhoneNumber = UserProfile.PhoneNumber;
                currentUser.Address = UserProfile.Address;

                await _userService.UpdateUserAsync(currentUser);

                // Update session variables if email or name changed
                HttpContext.Session.SetString("UserEmail", currentUser.Email);
                HttpContext.Session.SetString("UserName", currentUser.FullName);

                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user profile: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while updating your profile. Please try again.");
                return Page();
            }
        }
    }

    public class UserProfileModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;
    }
}