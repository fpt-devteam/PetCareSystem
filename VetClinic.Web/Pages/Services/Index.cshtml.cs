using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VetClinic.Repository.Entities;
using VetClinic.Service.Interfaces;
using VetClinic.Web.Helpers;

namespace VetClinic.Web.Pages.Services
{
    public class IndexModel : PageModel
    {
        private readonly IServiceService _serviceService;
        private readonly IUserService _userService;

        public IndexModel(IServiceService serviceService, IUserService userService)
        {
            _serviceService = serviceService;
            _userService = userService;
        }

        public IEnumerable<VetClinic.Repository.Entities.Service> Services { get; set; } = new List<VetClinic.Repository.Entities.Service>();
        public IEnumerable<User> Doctors { get; set; } = new List<User>();

        // Filter properties
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MinPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MaxPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool? ActiveFilter { get; set; }

        // Statistics properties (for Admin/Manager)
        public int ActiveServicesCount { get; set; }
        public int TotalServices { get; set; }
        public decimal AveragePrice { get; set; }
        public int AverageDuration { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await LoadServicesAsync();
                await LoadDoctorsAsync();
                await LoadStatsAsync();
                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading services and doctors. Please try again.";
                Console.WriteLine($"Error loading services: {ex.Message}");
                return Page();
            }
        }

        public async Task<IActionResult> OnPostToggleStatusAsync(int serviceId, bool isActive)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            // Only admins can edit services - consistent with Edit page
            if (!SessionHelper.IsInRole(HttpContext.Session, "Admin"))
            {
                TempData["ErrorMessage"] = "You are not authorized to modify services. Admin access required.";
                return RedirectToPage();
            }

            try
            {
                var userId = SessionHelper.GetUserId(HttpContext.Session);
                if (!userId.HasValue)
                {
                    TempData["ErrorMessage"] = "Unable to verify user session.";
                    return RedirectToPage();
                }

                // Check if service exists - consistent with Edit page
                var service = await _serviceService.GetServiceByIdAsync(serviceId);
                if (service == null)
                {
                    TempData["ErrorMessage"] = "Service not found.";
                    return RedirectToPage();
                }

                // Update the service status
                service.IsActive = isActive;
                await _serviceService.UpdateServiceAsync(service, userId.Value);

                var action = isActive ? "activated" : "deactivated";
                TempData["SuccessMessage"] = $"Service '{service.Name}' has been {action} successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error updating service status. Please try again.";
                Console.WriteLine($"Error toggling service status: {ex.Message}");
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int serviceId)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                var userRole = SessionHelper.GetUserRole(HttpContext.Session);
                if (userRole != "Admin")
                {
                    TempData["ErrorMessage"] = "You are not authorized to delete services. Admin access required.";
                    return RedirectToPage();
                }

                var userId = SessionHelper.GetUserId(HttpContext.Session);
                if (!userId.HasValue)
                {
                    TempData["ErrorMessage"] = "Unable to verify user session.";
                    return RedirectToPage();
                }

                await _serviceService.DeleteServiceAsync(serviceId, userId.Value);
                TempData["SuccessMessage"] = "Service has been deactivated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deactivating service. Please try again.";
                Console.WriteLine($"Error deleting service: {ex.Message}");
            }

            return RedirectToPage();
        }

        private async Task LoadServicesAsync()
        {
            try
            {
                var allServices = await _serviceService.GetAllServicesAsync();

                // Apply search filter
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    var searchLower = SearchTerm.ToLower();
                    allServices = allServices.Where(s =>
                        s.Name.ToLower().Contains(searchLower) ||
                        (s.Description?.ToLower().Contains(searchLower) ?? false));
                }

                // Apply price filters
                if (MinPrice.HasValue)
                {
                    allServices = allServices.Where(s => s.Price >= MinPrice.Value);
                }

                if (MaxPrice.HasValue)
                {
                    allServices = allServices.Where(s => s.Price <= MaxPrice.Value);
                }

                // Apply active status filter
                if (ActiveFilter.HasValue)
                {
                    allServices = allServices.Where(s => s.IsActive == ActiveFilter.Value);
                }

                Services = allServices.OrderBy(s => s.Name).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading services: {ex.Message}");
                Services = new List<VetClinic.Repository.Entities.Service>();
            }
        }

        private async Task LoadDoctorsAsync()
        {
            try
            {
                var doctors = await _userService.GetUsersByRoleAsync("Doctor");
                Doctors = doctors.OrderBy(d => d.FullName).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading doctors: {ex.Message}");
                Doctors = new List<User>();
            }
        }

        private async Task LoadStatsAsync()
        {
            try
            {
                var userRole = SessionHelper.GetUserRole(HttpContext.Session);

                // Only load stats for admin role
                if (userRole == "Admin")
                {
                    var allServices = await _serviceService.GetAllServicesAsync();
                    var serviceList = allServices.ToList();

                    TotalServices = serviceList.Count;
                    ActiveServicesCount = serviceList.Count(s => s.IsActive);
                    AveragePrice = serviceList.Any() ? serviceList.Average(s => s.Price) : 0;
                    AverageDuration = serviceList.Any() ? (int)serviceList.Average(s => s.DurationMinutes) : 0;
                }
                else
                {
                    // Initialize with zeros for non-admin users
                    TotalServices = ActiveServicesCount = AverageDuration = 0;
                    AveragePrice = 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading stats: {ex.Message}");
                TotalServices = ActiveServicesCount = AverageDuration = 0;
                AveragePrice = 0;
            }
        }
    }
}