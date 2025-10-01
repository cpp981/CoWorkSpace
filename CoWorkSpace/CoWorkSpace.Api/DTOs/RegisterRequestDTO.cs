using System.ComponentModel.DataAnnotations;

namespace CoWorkSpace.Api.DTOs
{
    public class RegisterRequestDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int RoleId { get; set; }
        public int? ProviderId { get; set; } // Opcional, solo para RoleId = 2
    }
}
