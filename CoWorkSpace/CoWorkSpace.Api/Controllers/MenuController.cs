using CoWorkSpace.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CoWorkSpace.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MenuController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<MenuItemDTO>> GetMenu()
        {
            // Obtener roleId desde el JWT
            var roleIdClaim = User.FindFirst("roleId")?.Value;
            if (!int.TryParse(roleIdClaim, out int roleId))
                return Forbid();

            List<MenuItemDTO> menuItems = roleId switch
            {
                1 => new List<MenuItemDTO> // SuperAdmin
                {
                    new MenuItemDTO { Label = "Dashboard", Action = "showDashboard", Icon = "bi bi-speedometer" },
                    new MenuItemDTO { Label = "Datos Negocio", Action = "showBusinessData", Icon = "bi bi-clipboard-data" },
                    new MenuItemDTO { Label = "Clientes", Action = "showClients", Icon = "bi bi-people-fill" },
                },
                2 => new List<MenuItemDTO> // Admin
                {
                    new MenuItemDTO { Label = "Dashboard", Action = "showDashboard", Icon = "bi bi-speedometer" },
                    new MenuItemDTO { Label = "Espacios", Action = "showSpaces", Icon = "bi bi-person-workspace" },
                    new MenuItemDTO { Label = "Clientes", Action = "showClients", Icon = "bi bi-people-fill" },
                },
                3 => new List<MenuItemDTO> // Provider
                {
                    new MenuItemDTO { Label = "Dashboard", Action = "showDashboard", Icon = "bi bi-speedometer" },
                    new MenuItemDTO { Label = "Espacios", Action = "showSpaces", Icon = "bi bi-person-workspace" },
                    new MenuItemDTO { Label = "Administradores", Action = "showAdmins", Icon = "bi bi-person-lines-fill" },
                },
                4 => new List<MenuItemDTO> // Client
                {
                    new MenuItemDTO { Label = "Dashboard", Action = "showDashboard", Icon = "bi bi-speedometer" },
                    new MenuItemDTO { Label = "Reservas", Action = "showBookings", Icon = "bi bi-calendar-day" },
                    new MenuItemDTO { Label = "Espacios", Action = "showSpaces", Icon = "bi bi-person-workspace" },
                },
                _ => new List<MenuItemDTO>() // Rol no reconocido
            };

            return Ok(menuItems);
        }
    }
}
