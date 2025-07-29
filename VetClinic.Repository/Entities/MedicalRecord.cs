using System.ComponentModel.DataAnnotations;

namespace VetClinic.Repository.Entities
{
    public class MedicalRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public int PetId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateTime VisitDate { get; set; }

        public string? ExaminationNotes { get; set; }

        public string? Diagnosis { get; set; }

        public string? TreatmentNotes { get; set; }

        public string? Prescription { get; set; }

        public string? FollowUpInstructions { get; set; }

        public DateTime? NextFollowUpDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Appointment Appointment { get; set; } = null!;
        public virtual Pet Pet { get; set; } = null!;
        public virtual User Doctor { get; set; } = null!;
        public virtual ICollection<LabResult> LabResults { get; set; } = new List<LabResult>();
    }
}
