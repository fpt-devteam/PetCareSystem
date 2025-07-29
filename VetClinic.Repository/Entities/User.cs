using System.ComponentModel.DataAnnotations;

namespace VetClinic.Repository.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(50)]
        public string Role { get; set; } = "Customer"; // Customer, Admin

        public bool IsActive { get; set; } = true;

        public DateTime? LastLoginDate { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();
        public virtual ICollection<Appointment> DoctorAppointments { get; set; } = new List<Appointment>();
        public virtual ICollection<MedicalRecord> DoctorMedicalRecords { get; set; } = new List<MedicalRecord>();
        public virtual ICollection<Feedback> CustomerFeedback { get; set; } = new List<Feedback>();
        public virtual ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
        public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
        public virtual ICollection<LabResult> UploadedLabResults { get; set; } = new List<LabResult>();
    }
}
