using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using VetClinic.Repository.Entities;
using VetClinic.Service.Interfaces;
using VetClinic.Web.Helpers;

namespace VetClinic.Web.Pages.Services
{
    public class CreateModel : PageModel
    {
        private readonly IServiceService _serviceService;

        public CreateModel(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        [BindProperty]
        public ServiceCreateModel Service { get; set; } = new ServiceCreateModel();

        public IActionResult OnGet()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            // Only admins and managers can create services
            if (!SessionHelper.IsInAnyRole(HttpContext.Session, new[] { "Admin", "Manager" }))
            {
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

            // Only admins and managers can create services
            if (!SessionHelper.IsInAnyRole(HttpContext.Session, new[] { "Admin", "Manager" }))
            {
                return RedirectToPage("/Index");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Check if service name is unique
                var existingService = await _serviceService.GetServiceByNameAsync(Service.Name);
                if (existingService != null)
                {
                    ModelState.AddModelError(nameof(Service.Name), "A service with this name already exists.");
                    return Page();
                }

                // Create the service entity
                var service = new VetClinic.Repository.Entities.Service
                {
                    Name = Service.Name,
                    Description = Service.Description,
                    DurationMinutes = Service.DurationMinutes,
                    Price = Service.Price,
                    IsActive = Service.IsActive,
                    CreatedDate = DateTime.UtcNow
                };

                await _serviceService.CreateServiceAsync(service);
                TempData["SuccessMessage"] = $"Service '{Service.Name}' has been created successfully.";
                return RedirectToPage("./Index");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating service: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while creating the service. Please try again.");
                return Page();
            }
        }
    }

    public class ServiceCreateModel
    {
        [Required(ErrorMessage = "Service name is required")]
        [StringLength(255, ErrorMessage = "Service name cannot exceed 255 characters")]
        [Display(Name = "Service Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [Range(15, 480, ErrorMessage = "Duration must be between 15 minutes and 8 hours")]
        [Display(Name = "Duration (Minutes)")]
        public int DurationMinutes { get; set; } = 30;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 9999.99, ErrorMessage = "Price must be between $0.01 and $9999.99")]
        [Display(Name = "Price ($)")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
}