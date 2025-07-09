using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using VetClinic.Service.Interfaces;
using VetClinic.Repository.Entities;

namespace VetClinic.Web.Pages.Users
{
    public class CreateModel : PageModel
    {
        private readonly IAuthService _authService;

        public CreateModel(IAuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public CreateUserInputModel Input { get; set; } = new();

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
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var user = await _authService.RegisterAsync(
                    Input.FullName,
                    Input.Email,
                    Input.Password,
                    Input.PhoneNumber,
                    Input.Address
                );

                TempData["SuccessMessage"] = "User created successfully!";
                return RedirectToPage("Index");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating the user.");
                return Page();
            }
        }
    }
}
