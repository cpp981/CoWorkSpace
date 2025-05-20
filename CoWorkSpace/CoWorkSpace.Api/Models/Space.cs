using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CoWorkSpace.Api.Models
{
    public class Space
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int AdminId { get; set; } // FK
        [ForeignKey("AdminId")]
        [JsonIgnore]
        public User Admin {  get; set; } // Vinculado a User con Role = 'Admin o SuperAdmin'
        public bool IsPublic { get; set; }
        public decimal Price { get; set; }
        public string City { get; set; }
        public List<Review> Reviews { get; set; }
        public List<Booking> Bookings { get; set; }
        public List<Payment> Payments { get; set; }
        public bool IsDeleted { get; set; } // Atributo para borrado lógico.
    }
}
