namespace CoWorkSpace.Api.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string TokenHash { get; set; } = null!; 
        public int UserId { get; set; }
        public DateTime Expires { get; set; }
        public bool Revoked { get; set; } = false;
        public DateTime? RevokedAt { get; set; }
        public string? ReplacedByTokenHash { get; set; } // hash del nuevo token
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedByIp { get; set; }
    }
}
