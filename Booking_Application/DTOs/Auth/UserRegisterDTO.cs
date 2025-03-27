using System.ComponentModel.DataAnnotations;

namespace Booking_Application.DTO_s.Auth
{
    public class UserRegisterDTO
    {
        [Required]
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Role { get; set; } = "User";

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
