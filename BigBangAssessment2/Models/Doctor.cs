using System.ComponentModel.DataAnnotations;

namespace BigBangAssessment2.Models
{
    public class Doctor
    {

        [Key]
        public int Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Phone { get; set; }

        public string? Clinic_Location { get; set; }

        public string? Fees { get; set; }

        public bool IsActive { get; set; }

        public ICollection<Patient>? Patients { get; set; }
    }
}
