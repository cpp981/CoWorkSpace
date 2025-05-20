using System.ComponentModel.DataAnnotations;


namespace CoWorkSpace.Api.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int? BookingId { get; set; }
        public Booking Booking { get; set; }
        public int? SpaceId { get; set; }
        public Space Space { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string Status { get; set; }
    }
}
