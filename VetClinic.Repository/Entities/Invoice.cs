using System.ComponentModel.DataAnnotations;
using VetClinic.Repository.Enums;

namespace VetClinic.Repository.Entities
{
    public class Invoice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = InvoiceStatus.Pending.ToString();

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? PaidDate { get; set; }

        // Navigation property
        public virtual Appointment Appointment { get; set; } = null!;
    }
}