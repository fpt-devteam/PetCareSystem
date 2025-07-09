using System.ComponentModel.DataAnnotations;
using VetClinic.Repository.Enums;

namespace VetClinic.Repository.Entities
{
    public class Vaccination
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PetId { get; set; }

        [Required]
        [StringLength(255)]
        public string VaccineName { get; set; } = string.Empty;

        [Required]
        public DateTime DueDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = VaccinationStatus.Due.ToString();

        public string? Notes { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation property
        public virtual Pet Pet { get; set; } = null!;
    }
}