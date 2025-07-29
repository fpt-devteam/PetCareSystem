using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using VetClinic.Service.Interfaces;
using VetClinic.Repository.Entities;
using VetClinic.Web.Helpers;

namespace VetClinic.Web.Pages.Users
{
    public class CreateModel : PageModel
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public CreateModel(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [BindProperty]
        public CreateUserInputModel Input { get; set; } = new();

        public SelectList RoleSelectList { get; set; } = new SelectList(new List<object>());

        public class CreateUserInputModel
        {
            [Required]
            [StringLength(100)]
            [Display(Name = "Full Name")]
            public string FullName { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Phone]
            [Display(Name = "Phone Number")]
            public string? PhoneNumber { get; set; }

            [StringLength(200)]
            [Display(Name = "Address")]
            public string? Address { get; set; }

            [Required]
            [StringLength(100, MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = string.Empty;

            [Required]
            [Display(Name = "Role")]
            public string Role { get; set; } = "Customer";
        }

        public IActionResult OnGet()
        {
            // Check authentication
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            // Check authorization - only Admin, Manager, or Staff can create users
            var currentUserRole = SessionHelper.GetUserRole(HttpContext.Session);
            if (!SessionHelper.IsInAnyRole(HttpContext.Session, "Admin", "Manager", "Staff"))
            {
                TempData["ErrorMessage"] = "You are not authorized to create users.";
                return RedirectToPage("/Dashboard");
            }

            LoadRoleOptions();
            return Page();
        }

        private void LoadRoleOptions()
        {
            var currentUserRole = SessionHelper.GetUserRole(HttpContext.Session);
            var availableRoles = new List<SelectListItem>();

            if (currentUserRole == "Admin" || currentUserRole == "Manager")
            {
                // Admin and Manager can create all roles
                availableRoles.Add(new SelectListItem { Value = "Customer", Text = "Customer" });
                availableRoles.Add(new SelectListItem { Value = "Staff", Text = "Staff" });
                availableRoles.Add(new SelectListItem { Value = "Doctor", Text = "Doctor" });
                
                if (currentUserRole == "Admin")
                {
                    availableRoles.Add(new SelectListItem { Value = "Manager", Text = "Manager" });
                }
            }
            else if (currentUserRole == "Staff")
            {
                // Staff can only create customers
                availableRoles.Add(new SelectListItem { Value = "Customer", Text = "Customer" });
            }

            RoleSelectList = new SelectList(availableRoles, "Value", "Text", "Customer");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Check authentication
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            // Check authorization
            var currentUserRole = SessionHelper.GetUserRole(HttpContext.Session);
            if (!SessionHelper.IsInAnyRole(HttpContext.Session, "Admin", "Manager", "Staff"))
            {
                TempData["ErrorMessage"] = "You are not authorized to create users.";
                return RedirectToPage("/Dashboard");
            }

            // Validate role permission
            var allowedRoles = GetAllowedRolesForCurrentUser(currentUserRole);
            if (!allowedRoles.Contains(Input.Role))
            {
                ModelState.AddModelError("Input.Role", "You are not authorized to create users with this role.");
                LoadRoleOptions();
                return Page();
            }

            if (!ModelState.IsValid)
            {
                LoadRoleOptions();
                return Page();
            }

            try
            {
                var user = await _authService.RegisterWithRoleAsync(
                    Input.FullName,
                    Input.Email,
                    Input.Password,
                    Input.Role,
                    Input.PhoneNumber,
                    Input.Address
                );

                TempData["SuccessMessage"] = $"User created successfully with role {Input.Role}!";
                return RedirectToPage("Index");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                LoadRoleOptions();
                return Page();
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating the user.");
                LoadRoleOptions();
                return Page();
            }
        }

        private List<string> GetAllowedRolesForCurrentUser(string? currentUserRole)
        {
            var allowedRoles = new List<string>();

            if (currentUserRole == "Admin")
            {
                allowedRoles.AddRange(new[] { "Customer", "Staff", "Doctor", "Manager" });
            }
            else if (currentUserRole == "Manager")
            {
                allowedRoles.AddRange(new[] { "Customer", "Staff", "Doctor" });
            }
            else if (currentUserRole == "Staff")
            {
                allowedRoles.Add("Customer");
            }

            return allowedRoles;
        }
    }
}
