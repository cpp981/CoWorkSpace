using CoWorkSpace.Api.DTOs;
using System.Collections.Generic;

namespace CoWorkSpace.Api.Services.Interfaces
{
    public interface IMenuService
    {
        IEnumerable<MenuItemDTO> GetMenuByRole(int roleId);
    }
}