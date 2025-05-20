using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CoWorkSpace.Api.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int SpaceId { get; set; }
        public Space Space { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        public List<Payment> Payments { get; set; }
    }
}
