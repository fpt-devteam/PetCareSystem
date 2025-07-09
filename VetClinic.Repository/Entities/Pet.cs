using System.ComponentModel.DataAnnotations;

namespace VetClinic.Repository.Entities
{
    public class Pet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OwnerId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Species { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Breed { get; set; }

        public DateTime? BirthDate { get; set; }

        public decimal? Weight { get; set; }

        [StringLength(20)]
        public string? Gender { get; set; }

        [StringLength(100)]
        public string? Color { get; set; }

        [StringLength(50)]
        public string? MicrochipId { get; set; }

        [StringLength(1000)]
        public string? MedicalNotes { get; set; }

        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string? PhotoUrl { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation property
        public virtual User Owner { get; set; } = null!;
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public virtual ICollection<Vaccination> Vaccinations { get; set; } = new List<Vaccination>();
    }
}