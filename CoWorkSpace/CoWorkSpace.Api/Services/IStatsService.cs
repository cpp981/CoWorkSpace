namespace CoWorkSpace.Api.Services
{
    public interface IStatsService
    {
        Task<UserStats> GetUserStats();
        Task<List<RegistrationStats>> GetRegistrationStats();
        Task<SpaceStats> GetSpaceStats();
        Task<AdminUserStats> GetAdminUserStats();
        Task<AdminBookingStats> GetAdminBookingStats();
        Task<ProviderSpaceStats> GetProviderSpaceStats(int providerId);
        Task<ProviderBookingStats> GetProviderBookingStats(int providerId);
        Task<ProviderOccupancyStats> GetProviderOccupancyStats(int providerId);
        Task<ClientBookingStats> GetClientBookingStats(string userId);
        Task<ClientActivityStats> GetClientActivityStats(string userId);
        Task<bool> IsProviderAuthorized(string userId, int providerId);
    }

    public class UserStats
    {
        public int TotalUsers { get; set; }
        public Dictionary<string, int> UsersByRole { get; set; }
    }

    public class RegistrationStats
    {
        public string Month { get; set; }
        public int Count { get; set; }
    }

    public class SpaceStats
    {
        public int TotalSpaces { get; set; }
        public List<ProviderSpace> SpacesByProvider { get; set; }
    }

    public class ProviderSpace
    {
        public int ProviderId { get; set; }
        public int Spaces { get; set; }
    }

    public class AdminUserStats
    {
        public int TotalUsers { get; set; }
    }

    public class AdminBookingStats
    {
        public int TotalBookings { get; set; }
        public List<MonthlyBookings> BookingsByMonth { get; set; }
    }

    public class MonthlyBookings
    {
        public string Month { get; set; }
        public int Count { get; set; }
    }

    public class ProviderSpaceStats
    {
        public int TotalSpaces { get; set; }
    }

    public class ProviderBookingStats
    {
        public int TotalBookings { get; set; }
        public List<DailyBookings> BookingsByDay { get; set; }
    }

    public class DailyBookings
    {
        public string Date { get; set; }
        public int Count { get; set; }
    }

    public class ProviderOccupancyStats
    {
        public double OccupancyRate { get; set; }
        public List<DailyOccupancy> OccupancyByDay { get; set; }
    }

    public class DailyOccupancy
    {
        public string Date { get; set; }
        public double Rate { get; set; }
    }

    public class ClientBookingStats
    {
        public int TotalBookings { get; set; }
        public List<BookingDetail> Bookings { get; set; }
    }

    public class BookingDetail
    {
        public string SpaceId { get; set; }
        public string Date { get; set; }
    }

    public class ClientActivityStats
    {
        public int TotalActions { get; set; }
        public List<ActionDetail> Actions { get; set; }
    }

    public class ActionDetail
    {
        public string Date { get; set; }
        public string ActionType { get; set; }
    }
}
