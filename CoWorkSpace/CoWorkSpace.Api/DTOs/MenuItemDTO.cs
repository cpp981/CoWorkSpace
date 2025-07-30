namespace CoWorkSpace.Api.DTOs
{
    public class MenuItemDTO
    {
        public string Label { get; set; }           // Texto visible
        public string Action { get; set; }          // Acción o nombre interno
        public string Icon { get; set; }            // Nombre del icono (para Lucide, FontAwesome, etc.)
    }
}
