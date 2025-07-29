using System.ComponentModel.DataAnnotations;

namespace VetClinic.Repository.Entities
{
    public class BlockedSlot
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        [StringLength(50)]
        public string BlockType { get; set; } = string.Empty; // FullDay, Morning, Afternoon, Custom

        [StringLength(500)]
        public string? Reason { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        // Navigation property
        public virtual User Doctor { get; set; } = null!;
    }
}
