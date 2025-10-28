using CoWorkSpace.Api.DTOs;
using CoWorkSpace.Api.Services.Interfaces;
using System.Collections.Generic;

namespace CoWorkSpace.Api.Services.Implementations
{
    public class MenuService : IMenuService
    {
        public IEnumerable<MenuItemDTO> GetMenuByRole(int roleId)
        {
            return roleId switch
            {
                1 => new List<MenuItemDTO>
                {
                    new() { Label = "Dashboard", Action = "showDashboard", Icon = "bi bi-speedometer" },
                    new() { Label = "Datos Negocio", Action = "showBusinessData", Icon = "bi bi-clipboard-data" },
                    new() { Label = "Clientes", Action = "showClients", Icon = "bi bi-people-fill" },
                },
                2 => new List<MenuItemDTO>
                {
                    new() { Label = "Dashboard", Action = "showDashboard", Icon = "bi bi-speedometer" },
                    new() { Label = "Espacios", Action = "showSpaces", Icon = "bi bi-person-workspace" },
                    new() { Label = "Clientes", Action = "showClients", Icon = "bi bi-people-fill" },
                },
                3 => new List<MenuItemDTO>
                {
                    new() { Label = "Dashboard", Action = "showDashboard", Icon = "bi bi-speedometer" },
                    new() { Label = "Espacios", Action = "showSpaces", Icon = "bi bi-person-workspace" },
                    new() { Label = "Administradores", Action = "showAdmins", Icon = "bi bi-person-lines-fill" },
                },
                4 => new List<MenuItemDTO>
                {
                    new() { Label = "Dashboard", Action = "showDashboard", Icon = "bi bi-speedometer" },
                    new() { Label = "Reservas", Action = "showBookings", Icon = "bi bi-calendar-day" },
                    new() { Label = "Espacios", Action = "showSpaces", Icon = "bi bi-person-workspace" },
                },
                _ => new List<MenuItemDTO>()
            };
        }
    }
}
