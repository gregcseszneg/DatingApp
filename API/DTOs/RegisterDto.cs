using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {
        [Required]
        [StringLength(16, MinimumLength = 4)]
        public string UserName { get; set; }

        [Required]
        [StringLength(12)]
        public string KnownAs { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public DateOnly? DateOfBirth { get; set; } //optional to make required work!

        [Required]
        [StringLength(64)]
        public string City { get; set; }

        [Required]
        [StringLength(64)]
        public string Country { get; set; }

        [Required]
        [StringLength(16, MinimumLength = 5)]
        public string Password { get; set; }
    }
}
