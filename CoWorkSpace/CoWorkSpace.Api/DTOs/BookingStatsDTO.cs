namespace CoWorkSpace.Api.DTOs
{
    public class BookingStatsDTO
    {
        public int BookingId { get; set; }
        public int SpaceId { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public decimal Amount { get; set; }
    }
}
