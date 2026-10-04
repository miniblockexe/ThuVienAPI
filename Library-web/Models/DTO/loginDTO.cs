using System.ComponentModel.DataAnnotations;

namespace Library_web.Models.DTO
{
    public class loginDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class loginResponseDTO
    {
        public string? JwtToken { get; set; }
    }
}