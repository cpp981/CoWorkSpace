namespace CoWorkSpace.Api.DTOs
{
    public class ProviderSpaceStatsDTO
    {
        public int SpaceId { get; set; }
        public string SpaceName { get; set; }
        public int AdminId { get; set; }
        public string AdminName { get; set; }
        public int BookingsCount { get; set; }
        public decimal Revenue { get; set; }
    }
}
