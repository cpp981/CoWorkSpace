using CoWorkSpace.Api.Models;
using System.Collections.Generic;

namespace CoWorkSpace.Api.DTOs
{
    public class AdminStatsDTO
    {
         public int TotalSpaces { get; set; }
         public int TotalBookings { get; set; }
         public decimal TotalRevenue { get; set; }
         public double AverageRating { get; set; }
         public List<SpaceStatsDTO> Spaces { get; set; }
    }
}
