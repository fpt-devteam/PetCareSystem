using System.ComponentModel.DataAnnotations;
using VetClinic.Repository.Enums;

namespace VetClinic.Repository.Entities
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PetId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [Required]
        public DateTime AppointmentTime { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = AppointmentStatus.Scheduled.ToString();

        public string? Notes { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Pet Pet { get; set; } = null!;
        public virtual User Doctor { get; set; } = null!;
        public virtual Service Service { get; set; } = null!;
        public virtual MedicalRecord? MedicalRecord { get; set; }
        public virtual ICollection<Feedback> Feedback { get; set; } = new List<Feedback>();
        public virtual Invoice? Invoice { get; set; }
    }
}