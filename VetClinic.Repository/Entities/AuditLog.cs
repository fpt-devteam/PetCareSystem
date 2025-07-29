using System.ComponentModel.DataAnnotations;

namespace VetClinic.Repository.Entities
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string UserRole { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Action { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string EntityType { get; set; } = string.Empty;

        public int? EntityId { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(45)]
        public string? IpAddress { get; set; }

        [StringLength(500)]
        public string? UserAgent { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        [StringLength(1000)]
        public string? AdditionalData { get; set; }

        // Navigation property
        public virtual User User { get; set; } = null!;
    }
}
