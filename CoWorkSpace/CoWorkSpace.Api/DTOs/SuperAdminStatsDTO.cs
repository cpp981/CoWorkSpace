namespace CoWorkSpace.Api.DTOs
{
    public class SuperAdminStatsDTO
    {
        public int TotalSpaces { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalUsers { get; set; }
        public Dictionary<string, int> UsersByRole { get; set; }
    }
}
