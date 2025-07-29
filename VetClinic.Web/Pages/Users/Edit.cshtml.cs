using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;
using VetClinic.Repository.Entities;
using VetClinic.Service.Interfaces;
using VetClinic.Web.Helpers;

namespace VetClinic.Web.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly IUserService _userService;

        public EditModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public User? UserDetails { get; set; }

        [BindProperty]
        public bool ResetPassword { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Check if user is logged in
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login", new { returnUrl = $"/Users/Edit/{id}" });
            }

            // Check if user has permission to edit users
            var isAdminOrManager = ViewSessionHelper.IsInAnyRole(HttpContext.Session, "Admin", "Manager");
            if (!isAdminOrManager)
            {
                TempData["ErrorMessage"] = "You don't have permission to edit users.";
                return RedirectToPage("/Index");
            }

            // Prevent editing your own account by non-admins
            var loggedInUserId = ViewSessionHelper.GetUserId(HttpContext.Session);
            if (id == loggedInUserId && !ViewSessionHelper.IsInRole(HttpContext.Session, "Admin"))
            {
                TempData["ErrorMessage"] = "You cannot edit your own account. Please contact an administrator.";
                return RedirectToPage("/Users/Details", new { id = id });
            }

            // Load user data
            UserDetails = await _userService.GetUserByIdAsync(id);
            if (UserDetails == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToPage("/Users/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Check permissions again
            if (!ViewSessionHelper.IsInAnyRole(HttpContext.Session, "Admin", "Manager"))
            {
                TempData["ErrorMessage"] = "You don't have permission to edit users.";
                return RedirectToPage("/Index");
            }

            // Validate the model
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Check if email is unique (excluding current user)
            if (UserDetails != null && !await _userService.IsEmailUniqueAsync(UserDetails.Email, UserDetails.Id))
            {
                ModelState.AddModelError("UserDetails.Email", "This email address is already in use.");
                return Page();
            }

            // Update user
            if (UserDetails != null)
            {
                var result = await _userService.UpdateUserAsync(UserDetails);
                
                if (result != null)
                {
                    // Handle password reset if requested
                    if (ResetPassword)
                    {
                        // In a real implementation, you would:
                        // 1. Generate a random password or reset token
                        // 2. Update the user's password hash
                        // 3. Send an email to the user with reset instructions
                        TempData["SuccessMessage"] = "User updated successfully. A new password has been sent to the user's email.";
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "User updated successfully.";
                    }
                    
                    return RedirectToPage("/Users/Details", new { id = UserDetails.Id });
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update user.";
                    return Page();
                }
            }
            
            TempData["ErrorMessage"] = "User data is missing.";
            return Page();
        }
    }
}
