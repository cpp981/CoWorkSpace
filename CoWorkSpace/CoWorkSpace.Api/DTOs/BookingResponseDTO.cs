namespace CoWorkSpace.Api.DTOs
{
    public class BookingResponseDTO
    {
        public int Id { get; set; }

        // Space
        public int SpaceId { get; set; }
        public string SpaceName { get; set; } = string.Empty;

        // User (cliente)
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;

        // Tiempos
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
