using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VetClinic.Service.Interfaces;
using VetClinic.Web.Helpers;

namespace VetClinic.Web.Pages.Reports
{
    public class IndexModel : PageModel
    {
        private readonly IDashboardService _dashboardService;
        private readonly IAppointmentService _appointmentService;
        private readonly IPetService _petService;
        private readonly IUserService _userService;

        public IndexModel(
            IDashboardService dashboardService,
            IAppointmentService appointmentService,
            IPetService petService,
            IUserService userService)
        {
            _dashboardService = dashboardService;
            _appointmentService = appointmentService;
            _petService = petService;
            _userService = userService;
        }

        // Analytics Properties
        public int TotalPets { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalAppointments { get; set; }
        public decimal TotalRevenue { get; set; }
        public double AverageRating { get; set; }

        // Appointment Statistics
        public int TodayAppointments { get; set; }
        public int WeekAppointments { get; set; }
        public int MonthAppointments { get; set; }

        // Monthly Revenue Data (for charts)
        public List<MonthlyRevenueData> MonthlyRevenue { get; set; } = new List<MonthlyRevenueData>();

        // Appointment Status Distribution
        public Dictionary<string, int> AppointmentStatusData { get; set; } = new Dictionary<string, int>();

        // Top Services Data
        public List<ServiceUsageData> TopServices { get; set; } = new List<ServiceUsageData>();

        // Pet Species Distribution
        public Dictionary<string, int> PetSpeciesData { get; set; } = new Dictionary<string, int>();

        // Filter Properties
        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ReportType { get; set; } = "overview";

        public async Task<IActionResult> OnGetAsync()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            // Only admins and managers can view reports
            if (!SessionHelper.IsInAnyRole(HttpContext.Session, new[] { "Admin", "Manager" }))
            {
                return RedirectToPage("/Index");
            }

            // Set default date range if not provided
            if (!StartDate.HasValue)
                StartDate = DateTime.Now.AddMonths(-1);
            if (!EndDate.HasValue)
                EndDate = DateTime.Now;

            try
            {
                await LoadReportDataAsync();
                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading reports: {ex.Message}");
                TempData["ErrorMessage"] = "Error loading reports. Please try again.";
                return Page();
            }
        }

        private async Task LoadReportDataAsync()
        {
            try
            {
                // Load basic statistics
                TotalPets = await _dashboardService.GetTotalPetsAsync();
                TotalCustomers = await _dashboardService.GetTotalCustomersAsync();
                TotalDoctors = await _dashboardService.GetTotalDoctorsAsync();
                TotalAppointments = await _dashboardService.GetTotalAppointmentsAsync(StartDate, EndDate);
                TotalRevenue = await _dashboardService.GetTotalRevenueAsync(StartDate, EndDate);
                AverageRating = await _dashboardService.GetAverageRatingAsync();

                // Load appointment statistics
                TodayAppointments = await _dashboardService.GetTotalAppointmentsAsync(DateTime.Today, DateTime.Today.AddDays(1));
                WeekAppointments = await _dashboardService.GetTotalAppointmentsAsync(DateTime.Now.AddDays(-7), DateTime.Now);
                MonthAppointments = await _dashboardService.GetTotalAppointmentsAsync(DateTime.Now.AddDays(-30), DateTime.Now);

                // Load monthly revenue data (last 12 months)
                await LoadMonthlyRevenueDataAsync();

                // Load appointment status distribution
                await LoadAppointmentStatusDataAsync();

                // Load pet species distribution
                await LoadPetSpeciesDataAsync();

                // Load top services data
                await LoadTopServicesDataAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LoadReportDataAsync: {ex.Message}");
                // Set safe defaults
                TotalPets = TotalCustomers = TotalDoctors = TotalAppointments = 0;
                TotalRevenue = 0m;
                AverageRating = 0.0;
                TodayAppointments = WeekAppointments = MonthAppointments = 0;
            }
        }

        private async Task LoadMonthlyRevenueDataAsync()
        {
            try
            {
                var revenueData = await _dashboardService.GetRevenueByMonthAsync(DateTime.Now.Year);
                MonthlyRevenue = revenueData.Cast<object>().Select(item => new MonthlyRevenueData
                {
                    Month = "Unknown", // You'd extract this from the dynamic object
                    Revenue = 0m // You'd extract this from the dynamic object
                }).ToList();

                // Fallback: Generate sample data if service doesn't return proper data
                if (!MonthlyRevenue.Any())
                {
                    for (int i = 11; i >= 0; i--)
                    {
                        var date = DateTime.Now.AddMonths(-i);
                        MonthlyRevenue.Add(new MonthlyRevenueData
                        {
                            Month = date.ToString("MMM yyyy"),
                            Revenue = await _dashboardService.GetTotalRevenueAsync(
                                new DateTime(date.Year, date.Month, 1),
                                new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1))
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading monthly revenue: {ex.Message}");
                MonthlyRevenue = new List<MonthlyRevenueData>();
            }
        }

        private async Task LoadAppointmentStatusDataAsync()
        {
            try
            {
                var appointments = await _appointmentService.GetAllAppointmentsAsync();
                AppointmentStatusData = appointments
                    .GroupBy(a => a.Status)
                    .ToDictionary(g => g.Key, g => g.Count());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading appointment status data: {ex.Message}");
                AppointmentStatusData = new Dictionary<string, int>();
            }
        }

        private async Task LoadPetSpeciesDataAsync()
        {
            try
            {
                var pets = await _petService.GetAllPetsAsync();
                PetSpeciesData = pets
                    .GroupBy(p => p.Species)
                    .ToDictionary(g => g.Key, g => g.Count());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading pet species data: {ex.Message}");
                PetSpeciesData = new Dictionary<string, int>();
            }
        }

        private async Task LoadTopServicesDataAsync()
        {
            try
            {
                var appointments = await _appointmentService.GetAllAppointmentsAsync();
                TopServices = appointments
                    .Where(a => a.Service != null)
                    .GroupBy(a => a.Service!.Name)
                    .Select(g => new ServiceUsageData
                    {
                        ServiceName = g.Key,
                        Count = g.Count(),
                        Revenue = g.Sum(a => a.Service!.Price)
                    })
                    .OrderByDescending(s => s.Count)
                    .Take(10)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading top services data: {ex.Message}");
                TopServices = new List<ServiceUsageData>();
            }
        }
    }

    public class MonthlyRevenueData
    {
        public string Month { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }

    public class ServiceUsageData
    {
        public string ServiceName { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Revenue { get; set; }
    }
}