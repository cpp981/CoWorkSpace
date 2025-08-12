using System;

namespace CoWorkSpace.Api.Models
{
    public class SpaceMembership
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int SpaceId { get; set; }
        public Space Space { get; set; }

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        public bool IsApproved { get; set; } = false;

        public DateTime? ApprovedAt { get; set; }
    }
}
