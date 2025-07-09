using VetClinic.Repository.Repositories;
using VetClinic.Service.Interfaces;

namespace VetClinic.Service.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IPetRepository _petRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IVaccinationRepository _vaccinationRepository;
        private readonly IFeedbackRepository _feedbackRepository;

        public DashboardService(
            IPetRepository petRepository,
            IAppointmentRepository appointmentRepository,
            IUserRepository userRepository,
            IInvoiceRepository invoiceRepository,
            IVaccinationRepository vaccinationRepository,
            IFeedbackRepository feedbackRepository)
        {
            _petRepository = petRepository;
            _appointmentRepository = appointmentRepository;
            _userRepository = userRepository;
            _invoiceRepository = invoiceRepository;
            _vaccinationRepository = vaccinationRepository;
            _feedbackRepository = feedbackRepository;
        }

        public async Task<int> GetTotalPetsAsync()
        {
            var pets = await _petRepository.GetAllAsync();
            return pets.Count();
        }

        public async Task<int> GetTotalAppointmentsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var appointments = await _appointmentRepository.GetAllAsync();

            if (startDate.HasValue && endDate.HasValue)
            {
                appointments = appointments.Where(a => a.AppointmentTime >= startDate && a.AppointmentTime <= endDate);
            }

            return appointments.Count();
        }

        public async Task<int> GetTotalCustomersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Count(u => u.Role == "Customer");
        }

        public async Task<int> GetTotalDoctorsAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Count(u => u.Role == "Doctor");
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                if (startDate.HasValue && endDate.HasValue)
                {
                    return await _invoiceRepository.GetTotalRevenueAsync(startDate.Value, endDate.Value);
                }

                // Default to current month if no dates provided
                var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                return await _invoiceRepository.GetTotalRevenueAsync(firstDayOfMonth, lastDayOfMonth);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting total revenue: {ex.Message}");
                return 0m;
            }
        }

        public async Task<double> GetAverageRatingAsync()
        {
            try
            {
                return await _feedbackRepository.GetAverageRatingAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting average rating: {ex.Message}");
                return 0.0;
            }
        }

        public async Task<object> GetDailySummaryAsync(DateTime date)
        {
            try
            {
                var appointments = await _appointmentRepository.GetAppointmentsForDateAsync(date);
                var revenue = await _invoiceRepository.GetTotalRevenueAsync(date, date.AddDays(1));

                return new
                {
                    Date = date.ToString("yyyy-MM-dd"),
                    TotalAppointments = appointments.Count(),
                    CompletedAppointments = appointments.Count(a => a.Status == "Completed"),
                    Revenue = revenue,
                    ScheduledAppointments = appointments.Count(a => a.Status == "Scheduled")
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting daily summary: {ex.Message}");
                return new
                {
                    Date = date.ToString("yyyy-MM-dd"),
                    TotalAppointments = 0,
                    CompletedAppointments = 0,
                    Revenue = 0m,
                    ScheduledAppointments = 0
                };
            }
        }

        public async Task<IEnumerable<object>> GetAppointmentsByDayAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var appointments = await _appointmentRepository.GetAllAsync();

                return appointments
                    .Where(a => a.AppointmentTime >= startDate && a.AppointmentTime <= endDate)
                    .GroupBy(a => a.AppointmentTime.Date)
                    .Select(g => new
                    {
                        Date = g.Key.ToString("yyyy-MM-dd"),
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Date);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting appointments by day: {ex.Message}");
                return new List<object>();
            }
        }

        public async Task<IEnumerable<object>> GetRevenueByMonthAsync(int year)
        {
            try
            {
                var startDate = new DateTime(year, 1, 1);
                var endDate = new DateTime(year, 12, 31);

                var invoices = await _invoiceRepository.GetInvoicesByDateRangeAsync(startDate, endDate);

                return invoices
                    .Where(i => i.Status == "Paid")
                    .GroupBy(i => new { i.CreatedDate.Year, i.CreatedDate.Month })
                    .Select(g => new
                    {
                        Month = $"{g.Key.Year}-{g.Key.Month:00}",
                        Revenue = g.Sum(i => i.TotalAmount)
                    })
                    .OrderBy(x => x.Month);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting revenue by month: {ex.Message}");
                return new List<object>();
            }
        }

        public async Task<int> GetDueVaccinationsCountAsync()
        {
            try
            {
                var vaccinations = await _vaccinationRepository.GetDueVaccinationsAsync();
                return vaccinations.Count();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting due vaccinations count: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> GetOverdueVaccinationsCountAsync()
        {
            try
            {
                var vaccinations = await _vaccinationRepository.GetOverdueVaccinationsAsync();
                return vaccinations.Count();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting overdue vaccinations count: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> GetUpcomingAppointmentsCountAsync(int days = 7)
        {
            try
            {
                var appointments = await _appointmentRepository.GetUpcomingAppointmentsAsync(days);
                return appointments.Count();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting upcoming appointments count: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> GetUnpaidInvoicesCountAsync()
        {
            try
            {
                var invoices = await _invoiceRepository.GetUnpaidInvoicesAsync();
                return invoices.Count();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting unpaid invoices count: {ex.Message}");
                return 0;
            }
        }

        public async Task<object> GetCustomerDashboardAsync(int customerId)
        {
            try
            {
                var pets = await _petRepository.GetPetsByOwnerIdAsync(customerId);
                var allAppointments = new List<VetClinic.Repository.Entities.Appointment>();

                foreach (var pet in pets)
                {
                    var petAppointments = await _appointmentRepository.GetAppointmentsByPetAsync(pet.Id);
                    allAppointments.AddRange(petAppointments);
                }

                var upcomingAppointments = allAppointments.Where(a =>
                    a.AppointmentTime > DateTime.Now &&
                    (a.Status == "Scheduled" || a.Status == "Pending")).Count();

                var recentAppointments = allAppointments
                    .OrderByDescending(a => a.AppointmentTime)
                    .Take(5)
                    .Select(a => new
                    {
                        Id = a.Id,
                        AppointmentTime = a.AppointmentTime,
                        Status = a.Status,
                        PetName = a.Pet?.Name ?? "Unknown"
                    });

                return new
                {
                    TotalPets = pets.Count(),
                    UpcomingAppointments = upcomingAppointments,
                    RecentAppointments = recentAppointments
                };
            }
            catch (Exception ex)
            {
                // Return safe defaults if there's an error
                Console.WriteLine($"Error in GetCustomerDashboardAsync: {ex.Message}");
                return new
                {
                    TotalPets = 0,
                    UpcomingAppointments = 0,
                    RecentAppointments = new List<object>()
                };
            }
        }

        public async Task<object> GetDoctorDashboardAsync(int doctorId)
        {
            try
            {
                var todayAppointments = await _appointmentRepository.GetAppointmentsByDoctorAsync(doctorId, DateTime.Today);
                var upcomingAppointments = await _appointmentRepository.GetUpcomingAppointmentsAsync(7);
                var doctorUpcoming = upcomingAppointments.Where(a => a.DoctorId == doctorId);

                return new
                {
                    TodayAppointments = todayAppointments.Count(),
                    UpcomingAppointments = doctorUpcoming.Count(),
                    TodaySchedule = todayAppointments.OrderBy(a => a.AppointmentTime).Take(10)
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetDoctorDashboardAsync: {ex.Message}");
                return new
                {
                    TodayAppointments = 0,
                    UpcomingAppointments = 0,
                    TodaySchedule = new List<object>()
                };
            }
        }

        public async Task<object> GetManagerDashboardAsync()
        {
            try
            {
                var totalRevenue = await GetTotalRevenueAsync();
                var totalAppointments = await GetTotalAppointmentsAsync();
                var totalCustomers = await GetTotalCustomersAsync();
                var averageRating = await GetAverageRatingAsync();

                return new
                {
                    TotalRevenue = totalRevenue,
                    TotalAppointments = totalAppointments,
                    TotalCustomers = totalCustomers,
                    AverageRating = averageRating,
                    DueVaccinations = await GetDueVaccinationsCountAsync(),
                    UnpaidInvoices = await GetUnpaidInvoicesCountAsync()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetManagerDashboardAsync: {ex.Message}");
                return new
                {
                    TotalRevenue = 0m,
                    TotalAppointments = 0,
                    TotalCustomers = 0,
                    AverageRating = 0.0,
                    DueVaccinations = 0,
                    UnpaidInvoices = 0
                };
            }
        }

        public async Task<object> GetStaffDashboardAsync()
        {
            try
            {
                var todayAppointments = await _appointmentRepository.GetAppointmentsForDateAsync(DateTime.Today);
                var upcomingAppointments = await GetUpcomingAppointmentsCountAsync(3);

                return new
                {
                    TodayAppointments = todayAppointments.Count(),
                    UpcomingAppointments = upcomingAppointments,
                    TodaySchedule = todayAppointments.OrderBy(a => a.AppointmentTime).Take(10)
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetStaffDashboardAsync: {ex.Message}");
                return new
                {
                    TodayAppointments = 0,
                    UpcomingAppointments = 0,
                    TodaySchedule = new List<object>()
                };
            }
        }
    }
}