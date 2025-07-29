using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VetClinic.Service.Interfaces;
using VetClinic.Web.Helpers;
using VetClinic.Repository.Entities;
using VetService = VetClinic.Repository.Entities.Service;

namespace VetClinic.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IDashboardService _dashboardService;
    private readonly IUserService _userService;
    private readonly IServiceService _serviceService;

    public IndexModel(
        ILogger<IndexModel> logger,
        IDashboardService dashboardService,
        IUserService userService,
        IServiceService serviceService)
    {
        _logger = logger;
        _dashboardService = dashboardService;
        _userService = userService;
        _serviceService = serviceService;
    }

    public object? DashboardData { get; set; }
    public string UserRole { get; set; } = string.Empty;

    // Landing page data
    public IEnumerable<VetService> Services { get; set; } = new List<VetService>();
    public IEnumerable<User> Doctors { get; set; } = new List<User>();
    public int TotalDoctors { get; set; }
    public int TotalServices { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalPets { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            // Load landing page data
            await LoadLandingPageDataAsync();

            // Check if user is authenticated for dashboard
            if (SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                UserRole = SessionHelper.GetUserRole(HttpContext.Session) ?? "Customer";
                var userId = SessionHelper.GetUserId(HttpContext.Session);

                if (userId.HasValue)
                {
                    DashboardData = UserRole switch
                    {
                        "Admin" or "Manager" => await _dashboardService.GetManagerDashboardAsync(),
                        "Doctor" => await _dashboardService.GetDoctorDashboardAsync(userId.Value),
                        "Staff" => await _dashboardService.GetStaffDashboardAsync(),
                        "Customer" => await _dashboardService.GetCustomerDashboardAsync(userId.Value),
                        _ => await _dashboardService.GetCustomerDashboardAsync(userId.Value)
                    };
                }
            }

            return Page();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading page data: {ex.Message}");
            TempData["ErrorMessage"] = "Unable to load page data. Please try refreshing the page.";
            return Page();
        }
    }

    private async Task LoadLandingPageDataAsync()
    {
        try
        {
            // Load active services
            var services = await _serviceService.GetActiveServicesAsync();
            Services = services.Take(6).ToList(); // Show only 6 services on landing page

            // Load doctors
            var doctors = await _userService.GetUsersByRoleAsync("Doctor");
            Doctors = doctors.Take(4).ToList(); // Show only 4 doctors on landing page

            // Load statistics
            TotalDoctors = await _dashboardService.GetTotalDoctorsAsync();
            TotalCustomers = await _dashboardService.GetTotalCustomersAsync();
            TotalPets = await _dashboardService.GetTotalPetsAsync();

            // Count total services
            var allServices = await _serviceService.GetAllServicesAsync();
            TotalServices = allServices.Count();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading landing page data: {ex.Message}");
            // Set default values if data loading fails
            Services = new List<VetService>();
            Doctors = new List<User>();
            TotalDoctors = 0;
            TotalServices = 0;
            TotalCustomers = 0;
            TotalPets = 0;
        }
    }
}
