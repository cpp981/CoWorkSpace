using System;

namespace CoWorkSpace.Api.DTOs
{
    public class BookingForAdminDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string NombreCliente { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
