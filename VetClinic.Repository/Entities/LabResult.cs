using System.ComponentModel.DataAnnotations;

namespace VetClinic.Repository.Entities
{
    public class LabResult
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MedicalRecordId { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FileType { get; set; } = string.Empty;

        [Required]
        public long FileSize { get; set; }

        [StringLength(500)]
        public string? FilePath { get; set; }

        [StringLength(255)]
        public string? ContentType { get; set; }

        public byte[] FileContent { get; set; } = Array.Empty<byte>();

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? LabName { get; set; }

        public DateTime? TestDate { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.Now;

        [Required]
        public int UploadedBy { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual MedicalRecord MedicalRecord { get; set; } = null!;
        public virtual User UploadedByUser { get; set; } = null!;
    }
}
