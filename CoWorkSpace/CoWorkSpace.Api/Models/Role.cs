using System.ComponentModel.DataAnnotations;

namespace CoWorkSpace.Api.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        [Required]
        public string RoleName { get; set; }

        public ICollection<User> Users { get; set; }
    }
}


