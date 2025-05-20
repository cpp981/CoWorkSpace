using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CoWorkSpace.Api.Models
{
    [Table("Users")]
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string Name { get; set; }

        [Required]
        public int RoleId { get; set; }

        public Role Role { get; set; }

        // Solo para Admins: su proveedor
        public int? ProviderId { get; set; }

        [ForeignKey("ProviderId")]
        [JsonIgnore] // para evitar ciclo de serialización
        public User Provider { get; set; }

        // Solo para Providers: sus admins
        [InverseProperty("Provider")]
        [JsonIgnore] // si vas a serializar usuarios, evita bucles
        public ICollection<User> Admins { get; set; }
    }
}
