namespace CoWorkSpace.Api.DTOs
{
    public class ClientWithSpacesDTO
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public List<string> SpaceNames { get; set; } = new List<string>();
    }
}
