using System.ComponentModel.DataAnnotations;

namespace VetClinic.Repository.Entities
{
    public class Feedback
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }

        public bool Approved { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Appointment Appointment { get; set; } = null!;
        public virtual User Customer { get; set; } = null!;
    }
}