using System.ComponentModel.DataAnnotations;

namespace BigBangAssessment2.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Disease_Name { get; set; }
        public int? Age { get; set; }

        public bool? IsVisited_Before { get; set; }

        public int DoctorId { get; set; }

        public Doctor? Doctor { get; set; }

    }
}
