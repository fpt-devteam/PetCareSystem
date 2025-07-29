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

        // Properties for key metrics
        public int TotalPets { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalAppointments { get; set; }
        public decimal TotalRevenue { get; set; }
        public double AverageRating { get; set; }
        public int TodayAppointments { get; set; }
        public int WeekAppointments { get; set; }
        public int MonthAppointments { get; set; }

        // Properties for filters
        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }
        [BindProperty(SupportsGet = true)]
        public string ReportType { get; set; } = "overview";

        // Properties for specific reports
        public DailySummaryData DailySummary { get; set; } = new DailySummaryData();
        public DoctorPerformanceData DoctorPerformance { get; set; }
        public PetHealthTimelineData PetHealthTimeline { get; set; }
        public List<VetClinic.Repository.Entities.User> Doctors { get; set; } = new List<VetClinic.Repository.Entities.User>();
        public List<VetClinic.Repository.Entities.Pet> Pets { get; set; } = new List<VetClinic.Repository.Entities.Pet>();
        [BindProperty(SupportsGet = true)]
        public int? DoctorId { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? PetId { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime? YearMonth { get; set; }

        // Properties for chart data
        public List<MonthlyRevenueData> MonthlyRevenue { get; set; } = new List<MonthlyRevenueData>();
        public AppointmentStatusData AppointmentStatus { get; set; } = new AppointmentStatusData();
        public List<PetSpeciesData> PetSpecies { get; set; } = new List<PetSpeciesData>();
        public List<TopServiceData> TopServices { get; set; } = new List<TopServiceData>();

        public async Task<IActionResult> OnGetAsync()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            if (!SessionHelper.IsInAnyRole(HttpContext.Session, new[] { "Admin", "Manager" }))
            {
                TempData["ErrorMessage"] = "Access denied. Admin or Manager role required.";
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
                // Load daily summary (USS-28)
                var dailySummaryResult = await _dashboardService.GetDailySummaryAsync(DateTime.Today);
                DailySummary = dailySummaryResult as DailySummaryData ?? new DailySummaryData();

                // Load doctors and pets for dropdowns
                Doctors = (await _userService.GetUsersByRoleAsync("Doctor")).ToList();
                Pets = (await _petService.GetAllPetsAsync()).ToList();

                // Load doctor performance (USS-20)
                if (ReportType == "doctor-performance" && DoctorId.HasValue && YearMonth.HasValue)
                {
                    var doctorPerformanceResult = await _dashboardService.GetDoctorMonthlyPerformanceAsync(
                        DoctorId.Value, YearMonth.Value.Year, YearMonth.Value.Month);
                    DoctorPerformance = doctorPerformanceResult as DoctorPerformanceData;
                }

                // Load pet health timeline (USS-19)
                if (ReportType == "pet-health" && PetId.HasValue)
                {
                    var petHealthResult = await _dashboardService.GetPetHealthTimelineAsync(
                        PetId.Value, StartDate, EndDate);
                    PetHealthTimeline = petHealthResult as PetHealthTimelineData;
                }

                // Load key metrics
                TotalPets = await _dashboardService.GetTotalPetsAsync();
                TotalCustomers = await _dashboardService.GetTotalCustomersAsync();
                TotalDoctors = await _dashboardService.GetTotalDoctorsAsync();
                TotalAppointments = await _dashboardService.GetTotalAppointmentsAsync(StartDate, EndDate);
                TotalRevenue = await _dashboardService.GetTotalRevenueAsync(StartDate, EndDate);
                AverageRating = await _dashboardService.GetAverageRatingAsync();
                TodayAppointments = await _dashboardService.GetTotalAppointmentsAsync(DateTime.Today, DateTime.Today.AddDays(1));
                WeekAppointments = await _dashboardService.GetTotalAppointmentsAsync(DateTime.Now.AddDays(-7), DateTime.Now);
                MonthAppointments = await _dashboardService.GetTotalAppointmentsAsync(DateTime.Now.AddDays(-30), DateTime.Now);

                // Load chart data
                await LoadMonthlyRevenueDataAsync();
                await LoadAppointmentStatusDataAsync();
                await LoadPetSpeciesDataAsync();
                await LoadTopServicesDataAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LoadReportDataAsync: {ex.Message}");
                TempData["ErrorMessage"] = "Error loading report data. Please try again.";
                // Set safe defaults
                TotalPets = TotalCustomers = TotalDoctors = TotalAppointments = 0;
                TotalRevenue = 0m;
                AverageRating = 0.0;
                TodayAppointments = WeekAppointments = MonthAppointments = 0;
                DailySummary = new DailySummaryData();
                MonthlyRevenue = new List<MonthlyRevenueData>();
                AppointmentStatus = new AppointmentStatusData();
                PetSpecies = new List<PetSpeciesData>();
                TopServices = new List<TopServiceData>();
            }
        }

        private async Task LoadMonthlyRevenueDataAsync()
        {
            try
            {
                var revenueData = await _dashboardService.GetMonthlyRevenueAsync(StartDate, EndDate);
                MonthlyRevenue = revenueData as List<MonthlyRevenueData> ?? new List<MonthlyRevenueData>();
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
                var statusData = await _dashboardService.GetAppointmentStatusAsync(StartDate, EndDate);
                AppointmentStatus = statusData as AppointmentStatusData ?? new AppointmentStatusData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading appointment status: {ex.Message}");
                AppointmentStatus = new AppointmentStatusData();
            }
        }

        private async Task LoadPetSpeciesDataAsync()
        {
            try
            {
                var speciesData = await _dashboardService.GetPetSpeciesDistributionAsync();
                PetSpecies = speciesData as List<PetSpeciesData> ?? new List<PetSpeciesData>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading pet species: {ex.Message}");
                PetSpecies = new List<PetSpeciesData>();
            }
        }

        private async Task LoadTopServicesDataAsync()
        {
            try
            {
                var servicesData = await _dashboardService.GetTopServicesAsync(StartDate, EndDate);
                TopServices = servicesData as List<TopServiceData> ?? new List<TopServiceData>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading top services: {ex.Message}");
                TopServices = new List<TopServiceData>();
            }
        }

        // Data models
        public class DailySummaryData
        {
            public string Date { get; set; } = DateTime.Today.ToString("yyyy-MM-dd");
            public int TotalAppointments { get; set; }
            public int CompletedAppointments { get; set; }
            public decimal Revenue { get; set; }
            public int ScheduledAppointments { get; set; }
        }

        public class DoctorPerformanceData
        {
            public string DoctorName { get; set; } = string.Empty;
            public int TotalAppointments { get; set; }
            public int CompletedAppointments { get; set; }
            public int CancelledAppointments { get; set; }
            public decimal Revenue { get; set; }
            public double AverageRating { get; set; }
            public string Month { get; set; } = string.Empty;
        }

        public class PetHealthTimelineData
        {
            public string PetName { get; set; } = string.Empty;
            public List<WeightRecord> WeightHistory { get; set; } = new List<WeightRecord>();
            public List<VaccineRecord> VaccinationHistory { get; set; } = new List<VaccineRecord>();
            public DateRange DateRange { get; set; } = new DateRange();
        }

        public class WeightRecord
        {
            public DateTime Date { get; set; }
            public decimal Weight { get; set; }
        }

        public class VaccineRecord
        {
            public DateTime Date { get; set; }
            public string VaccineName { get; set; } = string.Empty;
        }

        public class DateRange
        {
            public string StartDate { get; set; } = string.Empty;
            public string EndDate { get; set; } = string.Empty;
        }

        public class MonthlyRevenueData
        {
            public string Month { get; set; } = string.Empty;
            public decimal Revenue { get; set; }
        }

        public class AppointmentStatusData
        {
            public int Scheduled { get; set; }
            public int Completed { get; set; }
            public int Cancelled { get; set; }
        }

        public class PetSpeciesData
        {
            public string Species { get; set; } = string.Empty;
            public int Count { get; set; }
        }

        public class TopServiceData
        {
            public string ServiceName { get; set; } = string.Empty;
            public int Count { get; set; }
        }
    }
}