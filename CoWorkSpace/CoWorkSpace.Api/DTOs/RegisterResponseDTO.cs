using System.ComponentModel.DataAnnotations;

namespace CoWorkSpace.Api.DTOs
{
    public class UserResponseDTO
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public int? ProviderId { get; set; } // null si no aplica
    }
}

