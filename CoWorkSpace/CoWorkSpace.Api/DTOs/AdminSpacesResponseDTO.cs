namespace CoWorkSpace.Api.DTOs
{
    public class AdminSpacesResponseDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string SiguienteReserva { get; set; } = string.Empty;
    }
}
