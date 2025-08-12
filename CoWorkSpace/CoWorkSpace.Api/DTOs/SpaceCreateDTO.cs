namespace CoWorkSpace.Api.DTOs
{
    public class SpaceCreateDTO
    {
        public string Name { get; set; }
        public int AdminId { get; set; }
        public bool IsPublic { get; set; }
        public decimal Price { get; set; }
        public string City { get; set; }
    }
}
