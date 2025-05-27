using CoWorkSpace.Api.Models;
using System.Collections.Generic;

namespace CoWorkSpace.Api.DTOs
{
    public class ProviderStatsDTO
    {
        public int TotalSpaces { get; set; }
        public int TotalAdmins { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<ProviderSpaceStatsDTO> Spaces { get; set; }
    }
}
