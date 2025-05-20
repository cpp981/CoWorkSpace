using System.ComponentModel.DataAnnotations;

namespace CoWorkSpace.Api.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int SpaceId { get; set; }
        public Space Space { get; set; }
        [Required]
        public int Rating {  get; set; }
        public string Comment { get; set; }
    }
}
