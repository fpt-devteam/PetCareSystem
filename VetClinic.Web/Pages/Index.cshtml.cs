using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VetClinic.Service.Interfaces;
using VetClinic.Web.Helpers;

namespace VetClinic.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IDashboardService _dashboardService;

    public IndexModel(ILogger<IndexModel> logger, IDashboardService dashboardService)
    {
        _logger = logger;
        _dashboardService = dashboardService;
    }

    public object? DashboardData { get; set; }
    public string UserRole { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        // Check if user is authenticated
        if (!SessionHelper.IsAuthenticated(HttpContext.Session))
        {
            return RedirectToPage("/Account/Login");
        }

        UserRole = SessionHelper.GetUserRole(HttpContext.Session) ?? "Customer";
        var userId = SessionHelper.GetUserId(HttpContext.Session);

        try
        {
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
            else
            {
                // Provide safe defaults if userId is null
                DashboardData = new
                {
                    TotalPets = 0,
                    UpcomingAppointments = 0,
                    TodayAppointments = 0,
                    TotalRevenue = 0m,
                    TotalCustomers = 0,
                    AverageRating = 0.0,
                    DueVaccinations = 0,
                    UnpaidInvoices = 0,
                    RecentAppointments = new List<object>(),
                    TodaySchedule = new List<object>()
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading dashboard data: {ex.Message}");
            TempData["ErrorMessage"] = "Unable to load dashboard data. Please try refreshing the page.";

            // Provide safe fallback data to prevent view errors
            DashboardData = new
            {
                TotalPets = 0,
                UpcomingAppointments = 0,
                TodayAppointments = 0,
                TotalRevenue = 0m,
                TotalCustomers = 0,
                AverageRating = 0.0,
                DueVaccinations = 0,
                UnpaidInvoices = 0,
                RecentAppointments = new List<object>(),
                TodaySchedule = new List<object>()
            };
        }

        return Page();
    }
}
