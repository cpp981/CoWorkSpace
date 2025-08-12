namespace CoWorkSpace.Api.DTOs.Provider
{
    public class ProviderSpaceDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AdminId { get; set; }
        public bool IsPublic { get; set; }
        public decimal Price { get; set; }
        public string City { get; set; }
        public bool IsActive { get; set; } // Derivado de !IsDeleted
    }
}
