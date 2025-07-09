using System.ComponentModel.DataAnnotations;

namespace VetClinic.Repository.Entities
{
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public int DurationMinutes { get; set; }

        [Required]
        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation property
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}