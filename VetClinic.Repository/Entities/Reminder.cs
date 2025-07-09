using System.ComponentModel.DataAnnotations;
using VetClinic.Repository.Enums;

namespace VetClinic.Repository.Entities
{
    public class Reminder
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public int? PetId { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        public string? Message { get; set; }

        [Required]
        public DateTime SendAt { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = ReminderStatus.Pending.ToString();

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual Pet? Pet { get; set; }
    }
}