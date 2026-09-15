using System.ComponentModel.DataAnnotations;

namespace ChatApp.DTOs.UserDTOs
{
    public class RegisterRequestDTO
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? UserEmail { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match.")]
        public string? Confirm { get; set; }
        [Required]
        public string? Role { get; set; }
    
    }
}
