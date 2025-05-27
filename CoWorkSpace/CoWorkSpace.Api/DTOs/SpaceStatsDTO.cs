namespace CoWorkSpace.Api.DTOs
{
    public class SpaceStatsDTO
    {
        public int SpaceId { get; set; }
        public string SpaceName { get; set; }
        public int BookingsCount { get; set; }
        public decimal Revenue { get; set; }
        public double AverageRating { get; set; }
    }
}
