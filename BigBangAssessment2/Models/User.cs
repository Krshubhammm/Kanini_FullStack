using System.ComponentModel.DataAnnotations;

namespace BigBangAssessment2.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Username { get; set; }
        public string? Password { get; set; }
         public string? Email { get; set; }
          
        public string? token { get; set; }
        public string? Role { get; set; }
        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }  
    }
}
