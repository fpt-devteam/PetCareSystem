using VetClinic.Repository.Repositories;
using VetClinic.Service.Interfaces;
using VetClinic.Service.Models;
using System.Diagnostics;
using VetClinic.Repository.Entities;

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
        private readonly IMedicalRecordRepository _medicalRecordRepository;

        public DashboardService(
            IPetRepository petRepository,
            IAppointmentRepository appointmentRepository,
            IUserRepository userRepository,
            IInvoiceRepository invoiceRepository,
            IVaccinationRepository vaccinationRepository,
            IFeedbackRepository feedbackRepository,
            IMedicalRecordRepository medicalRecordRepository)
        {
            _petRepository = petRepository;
            _appointmentRepository = appointmentRepository;
            _userRepository = userRepository;
            _invoiceRepository = invoiceRepository;
            _vaccinationRepository = vaccinationRepository;
            _feedbackRepository = feedbackRepository;
            _medicalRecordRepository = medicalRecordRepository;
        }

        public async Task<int> GetTotalPetsAsync()
        {
            var pets = await _petRepository.GetAllAsync();
            Debug.WriteLine($"GetTotalPetsAsync: Total pets count = {pets.Count()}");
            return pets.Count();
        }

        public async Task<int> GetTotalAppointmentsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var appointments = await _appointmentRepository.GetAllAsync();
            if (startDate.HasValue && endDate.HasValue)
            {
                appointments = appointments.Where(a => a.AppointmentTime >= startDate && a.AppointmentTime <= endDate);
            }
            Debug.WriteLine($"GetTotalAppointmentsAsync: Total appointments = {appointments.Count()}, StartDate = {startDate}, EndDate = {endDate}");
            return appointments.Count();
        }

        public async Task<int> GetTotalCustomersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var customerCount = users.Count(u => u.Role == "Customer");
            Debug.WriteLine($"GetTotalCustomersAsync: Total customers = {customerCount}");
            return customerCount;
        }

        public async Task<int> GetTotalDoctorsAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var doctorCount = users.Count(u => u.Role == "Doctor");
            Debug.WriteLine($"GetTotalDoctorsAsync: Total doctors = {doctorCount}");
            return doctorCount;
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                if (startDate.HasValue && endDate.HasValue)
                {
                    var revenue = await _invoiceRepository.GetTotalRevenueAsync(startDate.Value, endDate.Value);
                    Debug.WriteLine($"GetTotalRevenueAsync: Revenue = {revenue}, StartDate = {startDate}, EndDate = {endDate}");
                    return revenue;
                }

                var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                var defaultRevenue = await _invoiceRepository.GetTotalRevenueAsync(firstDayOfMonth, lastDayOfMonth);
                Debug.WriteLine($"GetTotalRevenueAsync: Default revenue = {defaultRevenue}, Default range = {firstDayOfMonth} to {lastDayOfMonth}");
                return defaultRevenue;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting total revenue: {ex.Message}");
                return 0m;
            }
        }

        public async Task<double> GetAverageRatingAsync()
        {
            try
            {
                var rating = await _feedbackRepository.GetAverageRatingAsync();
                Debug.WriteLine($"GetAverageRatingAsync: Average rating = {rating}");
                return rating;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting average rating: {ex.Message}");
                return 0.0;
            }
        }

        public async Task<object> GetDailySummaryAsync(DateTime date)
        {
            try
            {
                var appointments = await _appointmentRepository.GetAppointmentsForDateAsync(date);
                var revenue = await _invoiceRepository.GetTotalRevenueAsync(date, date.AddDays(1));
                var result = new
                {
                    Date = date.ToString("yyyy-MM-dd"),
                    TotalAppointments = appointments.Count(),
                    CompletedAppointments = appointments.Count(a => a.Status == "Completed"),
                    Revenue = revenue,
                    ScheduledAppointments = appointments.Count(a => a.Status == "Scheduled")
                };
                Debug.WriteLine($"GetDailySummaryAsync: Date = {result.Date}, TotalAppointments = {result.TotalAppointments}, CompletedAppointments = {result.CompletedAppointments}, Revenue = {result.Revenue}, ScheduledAppointments = {result.ScheduledAppointments}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting daily summary: {ex.Message}");
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
                var result = appointments
                    .Where(a => a.AppointmentTime >= startDate && a.AppointmentTime <= endDate)
                    .GroupBy(a => a.AppointmentTime.Date)
                    .Select(g => new
                    {
                        Date = g.Key.ToString("yyyy-MM-dd"),
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Date);
                Debug.WriteLine($"GetAppointmentsByDayAsync: StartDate = {startDate}, EndDate = {endDate}, Results = {string.Join(", ", result.Select(r => $"Date={r.Date}, Count={r.Count}"))}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting appointments by day: {ex.Message}");
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

                Debug.WriteLine($"[DEBUG] Total invoices received: {invoices.Count()}");

                var result = invoices
                    .Where(i => i.Status == "Paid")
                    .GroupBy(i => new { i.CreatedDate.Year, i.CreatedDate.Month })
                    .Select(g => new
                    {
                        Month = $"{g.Key.Year}-{g.Key.Month:00}",
                        Revenue = g.Sum(i => i.TotalAmount)
                    })
                    .OrderBy(x => x.Month);
                Debug.WriteLine($"GetRevenueByMonthAsync: Year = {year}, Results = {string.Join(", ", result.Select(r => $"Month={r.Month}, Revenue={r.Revenue}"))}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting revenue by month: {ex.Message}");
                return new List<object>();
            }
        }

        public async Task<int> GetDueVaccinationsCountAsync()
        {
            try
            {
                var vaccinations = await _vaccinationRepository.GetDueVaccinationsAsync();
                var count = vaccinations.Count();
                Debug.WriteLine($"GetDueVaccinationsCountAsync: Count = {count}");
                return count;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting due vaccinations count: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> GetOverdueVaccinationsCountAsync()
        {
            try
            {
                var vaccinations = await _vaccinationRepository.GetOverdueVaccinationsAsync();
                var count = vaccinations.Count();
                Debug.WriteLine($"GetOverdueVaccinationsCountAsync: Count = {count}");
                return count;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting overdue vaccinations count: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> GetUpcomingAppointmentsCountAsync(int days = 7)
        {
            try
            {
                var appointments = await _appointmentRepository.GetUpcomingAppointmentsAsync(days);
                var count = appointments.Count();
                Debug.WriteLine($"GetUpcomingAppointmentsCountAsync: Count = {count}, Days = {days}");
                return count;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting upcoming appointments count: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> GetUnpaidInvoicesCountAsync()
        {
            try
            {
                var invoices = await _invoiceRepository.GetUnpaidInvoicesAsync();
                var count = invoices.Count();
                Debug.WriteLine($"GetUnpaidInvoicesCountAsync: Count = {count}");
                return count;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting unpaid invoices count: {ex.Message}");
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
                var upcomingAppointments = allAppointments.Where(a => a.AppointmentTime > DateTime.Now && (a.Status == "Scheduled" || a.Status == "Pending")).Count();
                var recentAppointments = allAppointments.OrderByDescending(a => a.AppointmentTime).Take(5).Select(a => new
                {
                    Id = a.Id,
                    AppointmentTime = a.AppointmentTime,
                    Status = a.Status,
                    PetName = a.Pet?.Name ?? "Unknown"
                });
                var result = new
                {
                    TotalPets = pets.Count(),
                    UpcomingAppointments = upcomingAppointments,
                    RecentAppointments = recentAppointments
                };
                Debug.WriteLine($"GetCustomerDashboardAsync: CustomerId = {customerId}, TotalPets = {result.TotalPets}, UpcomingAppointments = {result.UpcomingAppointments}, RecentAppointments = {string.Join(", ", result.RecentAppointments.Select(r => $"Id={r.Id}, Time={r.AppointmentTime}, Status={r.Status}, PetName={r.PetName}"))}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetCustomerDashboardAsync: {ex.Message}");
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
                var result = new
                {
                    TodayAppointments = todayAppointments.Count(),
                    UpcomingAppointments = doctorUpcoming.Count(),
                    TodaySchedule = todayAppointments.OrderBy(a => a.AppointmentTime).Take(10)
                };
                Debug.WriteLine($"GetDoctorDashboardAsync: DoctorId = {doctorId}, TodayAppointments = {result.TodayAppointments}, UpcomingAppointments = {result.UpcomingAppointments}, TodaySchedule = {string.Join(", ", result.TodaySchedule.Select(a => $"Time={a.AppointmentTime}, Status={a.Status}"))}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetDoctorDashboardAsync: {ex.Message}");
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
                var result = new
                {
                    TotalRevenue = totalRevenue,
                    TotalAppointments = totalAppointments,
                    TotalCustomers = totalCustomers,
                    AverageRating = averageRating,
                    DueVaccinations = await GetDueVaccinationsCountAsync(),
                    UnpaidInvoices = await GetUnpaidInvoicesCountAsync()
                };
                Debug.WriteLine($"GetManagerDashboardAsync: TotalRevenue = {result.TotalRevenue}, TotalAppointments = {result.TotalAppointments}, TotalCustomers = {result.TotalCustomers}, AverageRating = {result.AverageRating}, DueVaccinations = {result.DueVaccinations}, UnpaidInvoices = {result.UnpaidInvoices}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetManagerDashboardAsync: {ex.Message}");
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
                var result = new
                {
                    TodayAppointments = todayAppointments.Count(),
                    UpcomingAppointments = upcomingAppointments,
                    TodaySchedule = todayAppointments.OrderBy(a => a.AppointmentTime).Take(10)
                };
                Debug.WriteLine($"GetStaffDashboardAsync: TodayAppointments = {result.TodayAppointments}, UpcomingAppointments = {result.UpcomingAppointments}, TodaySchedule = {string.Join(", ", result.TodaySchedule.Select(a => $"Time={a.AppointmentTime}, Status={a.Status}"))}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetStaffDashboardAsync: {ex.Message}");
                return new
                {
                    TodayAppointments = 0,
                    UpcomingAppointments = 0,
                    TodaySchedule = new List<object>()
                };
            }
        }

        public async Task<object> GetDoctorMonthlyPerformanceAsync(int doctorId, int year, int month)
        {
            try
            {
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                var doctor = await _userRepository.GetByIdAsync(doctorId);
                if (doctor == null || doctor.Role != "Doctor")
                    throw new ArgumentException("Doctor not found");
                var appointments = await _appointmentRepository.GetAppointmentsByDoctorAsync(doctorId, startDate);
                var monthlyAppointments = appointments.Where(a => a.AppointmentTime >= startDate && a.AppointmentTime <= endDate).ToList();
                var invoiceIds = monthlyAppointments.Select(a => a.Id).ToList();
                var invoices = await _invoiceRepository.GetInvoicesByDateRangeAsync(startDate, endDate);
                var monthlyRevenue = invoices.Where(i => invoiceIds.Contains(i.AppointmentId) && i.Status == "Paid").Sum(i => i.TotalAmount);
                var feedback = await _feedbackRepository.GetFeedbackByDoctorAsync(doctorId);
                var monthlyFeedback = feedback.Where(f => f.CreatedDate >= startDate && f.CreatedDate <= endDate);
                var averageRating = monthlyFeedback.Any() ? monthlyFeedback.Average(f => f.Rating) : 0;
                var result = new
                {
                    DoctorName = doctor.FullName,
                    TotalAppointments = monthlyAppointments.Count,
                    CompletedAppointments = monthlyAppointments.Count(a => a.Status == "Completed"),
                    CancelledAppointments = monthlyAppointments.Count(a => a.Status == "Cancelled"),
                    Revenue = monthlyRevenue,
                    AverageRating = averageRating,
                    Month = startDate.ToString("MMM yyyy")
                };
                Debug.WriteLine($"GetDoctorMonthlyPerformanceAsync: DoctorId = {doctorId}, Year = {year}, Month = {month}, DoctorName = {result.DoctorName}, TotalAppointments = {result.TotalAppointments}, CompletedAppointments = {result.CompletedAppointments}, CancelledAppointments = {result.CancelledAppointments}, Revenue = {result.Revenue}, AverageRating = {result.AverageRating}, Month = {result.Month}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting doctor monthly performance: {ex.Message}");
                return new
                {
                    DoctorName = "Unknown",
                    TotalAppointments = 0,
                    CompletedAppointments = 0,
                    CancelledAppointments = 0,
                    Revenue = 0m,
                    AverageRating = 0.0,
                    Month = $"{year}-{month:00}"
                };
            }
        }

        public async Task<object> GetPetHealthTimelineAsync(int petId, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var pet = await _petRepository.GetByIdAsync(petId);
                if (pet == null)
                    throw new ArgumentException("Pet not found");
                startDate ??= DateTime.Now.AddYears(-1);
                endDate ??= DateTime.Now;
                var medicalRecords = await _medicalRecordRepository.GetMedicalRecordsByPetAsync(petId);
                var vaccinations = await _vaccinationRepository.GetVaccinationsByPetAsync(petId);
                var weightRecords = medicalRecords
                    .Where(m => m.VisitDate >= startDate && m.VisitDate <= endDate && pet.Weight.HasValue)
                    .Select(m => new { Date = m.VisitDate, Weight = pet.Weight.Value })
                    .OrderBy(m => m.Date)
                    .ToList();
                var vaccineRecords = vaccinations
                    .Where(v => v.DueDate >= startDate && v.DueDate <= endDate)
                    .Select(v => new { Date = v.DueDate, VaccineName = v.VaccineName })
                    .OrderBy(v => v.Date)
                    .ToList();
                var result = new
                {
                    PetName = pet.Name,
                    WeightHistory = weightRecords,
                    VaccinationHistory = vaccineRecords,
                    DateRange = new { StartDate = startDate.Value.ToString("yyyy-MM-dd"), EndDate = endDate.Value.ToString("yyyy-MM-dd") }
                };
                Debug.WriteLine($"GetPetHealthTimelineAsync: PetId = {petId}, PetName = {result.PetName}, WeightHistory = {string.Join(", ", result.WeightHistory.Select(w => $"Date={w.Date}, Weight={w.Weight}"))}, VaccinationHistory = {string.Join(", ", result.VaccinationHistory.Select(v => $"Date={v.Date}, VaccineName={v.VaccineName}"))}, DateRange = StartDate={result.DateRange.StartDate}, EndDate={result.DateRange.EndDate}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting pet health timeline: {ex.Message}");
                return new
                {
                    PetName = "Unknown",
                    WeightHistory = new List<object>(),
                    VaccinationHistory = new List<object>(),
                    DateRange = new { StartDate = startDate?.ToString("yyyy-MM-dd") ?? DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd"), EndDate = endDate?.ToString("yyyy-MM-dd") ?? DateTime.Now.ToString("yyyy-MM-dd") }
                };
            }
        }

        public async Task<List<MonthlyRevenueData>> GetMonthlyRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                startDate ??= new DateTime(2025, 7, 1);
                endDate ??= new DateTime(2025, 7, 31);
                var invoices = await _invoiceRepository.GetInvoicesByDateRangeAsync(startDate.Value, endDate.Value);
                var filteredInvoices = invoices.Where(i => i.Status == "Paid" && i.PaidDate.HasValue).ToList();
                var result = filteredInvoices
                    .GroupBy(i => new { i.PaidDate.Value.Year, i.PaidDate.Value.Month })
                    .Select(g => new MonthlyRevenueData
                    {
                        Month = $"{g.Key.Year}-{g.Key.Month:00}",
                        Revenue = g.Sum(i => i.TotalAmount)
                    })
                    .OrderBy(x => x.Month)
                    .ToList();
                Debug.WriteLine($"GetMonthlyRevenueAsync: StartDate = {startDate}, EndDate = {endDate}, InvoicesCount = {invoices.Count()}, FilteredInvoicesCount = {filteredInvoices.Count()}, Results = {string.Join(", ", result.Select(r => $"Month={r.Month}, Revenue={r.Revenue}"))}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting monthly revenue: {ex.Message}");
                return new List<MonthlyRevenueData>();
            }
        }

        public async Task<AppointmentStatusData> GetAppointmentStatusAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                startDate ??= new DateTime(2025, 7, 1);
                endDate ??= new DateTime(2025, 7, 31);
                var appointments = await _appointmentRepository.GetAllAsync();
                appointments = appointments.Where(a => a.AppointmentTime >= startDate && a.AppointmentTime <= endDate);
                var result = new AppointmentStatusData
                {
                    Scheduled = appointments.Count(a => a.Status == "Scheduled"),
                    Completed = appointments.Count(a => a.Status == "Completed"),
                    Cancelled = appointments.Count(a => a.Status == "Cancelled")
                };
                Debug.WriteLine($"GetAppointmentStatusAsync: StartDate = {startDate}, EndDate = {endDate}, Scheduled = {result.Scheduled}, Completed = {result.Completed}, Cancelled = {result.Cancelled}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting appointment status: {ex.Message}");
                return new AppointmentStatusData();
            }
        }

        public async Task<List<PetSpeciesData>> GetPetSpeciesDistributionAsync()
        {
            try
            {
                var pets = await _petRepository.GetAllAsync();
                var result = pets
                    .GroupBy(p => p.Species)
                    .Select(g => new PetSpeciesData
                    {
                        Species = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .ToList();
                Debug.WriteLine($"GetPetSpeciesDistributionAsync: TotalPets = {pets.Count()}, Results = {string.Join(", ", result.Select(r => $"Species={r.Species}, Count={r.Count}"))}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting pet species distribution: {ex.Message}");
                return new List<PetSpeciesData>();
            }
        }

        public async Task<List<TopServiceData>> GetTopServicesAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                startDate ??= new DateTime(2025, 7, 1);
                endDate ??= new DateTime(2025, 7, 31);
                var appointments = await _appointmentRepository.GetAllAsync();
                appointments = appointments.Where(a => a.AppointmentTime >= startDate && a.AppointmentTime <= endDate);
                var result = appointments
                    .GroupBy(a => a.Service)
                    .Select(g => new TopServiceData
                    {
                        ServiceName = g.Key?.Name ?? "Unknown",
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .Take(5)
                    .ToList();
                Debug.WriteLine($"GetTopServicesAsync: StartDate = {startDate}, EndDate = {endDate}, TotalAppointments = {appointments.Count()}, Results = {string.Join(", ", result.Select(r => $"ServiceName={r.ServiceName}, Count={r.Count}"))}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting top services: {ex.Message}");
                return new List<TopServiceData>();
            }
        }
    }
}