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
    }
}
