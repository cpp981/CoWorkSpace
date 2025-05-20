using System.ComponentModel.DataAnnotations;



namespace CoWorkSpace.Api.Models
{
    public class Log
    {
        public int Id { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }
        [Required]
        public string EventType { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
        public string Details { get; set; }
    }
}
