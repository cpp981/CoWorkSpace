namespace CoWorkSpace.Api.DTOs
{
    public class BookingUpdateResultDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string Message { get; set; }
    }
}
