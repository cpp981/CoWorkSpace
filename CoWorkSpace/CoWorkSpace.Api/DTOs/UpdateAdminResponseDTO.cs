using System.ComponentModel.DataAnnotations;

namespace CoWorkSpace.Api.DTOs
{
    public class UpdateAdminResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int ProviderId { get; set; }
        public int RoleId { get; set; }
        public string Message { get; set; }
    }
}
