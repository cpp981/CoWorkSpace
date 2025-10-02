using System.ComponentModel.DataAnnotations;

namespace CoWorkSpace.Api.DTOs
{
    public class UpdateAdminRequestDTO
    { 
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")] 
        public string? Name { get; set; } 
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")] 
        public string? Email { get; set; } 
        public string? Password { get; set; } }

}
