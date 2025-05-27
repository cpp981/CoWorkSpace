using CoWorkSpace.Api.Models;
using System.Collections.Generic;

namespace CoWorkSpace.Api.DTOs
{
    public class ClientStatsDTO
    {
        public int TotalBookings { get; set; }
        public decimal TotalSpent { get; set; }
        public int TotalReviews { get; set; }
        public List<BookingStatsDTO> Bookings { get; set; }
    }
}
