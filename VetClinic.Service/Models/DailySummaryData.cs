using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetClinic.Service.Models
{
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
