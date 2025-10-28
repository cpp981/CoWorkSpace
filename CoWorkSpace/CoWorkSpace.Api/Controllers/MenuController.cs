// Controllers/MenuController.cs
using CoWorkSpace.Api.DTOs;
using CoWorkSpace.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoWorkSpace.Api.Controllers
{
    [ApiController]
    [Route("api/v1/menu")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<MenuItemDTO>> GetMenu()
        {
            var roleIdClaim = User.FindFirst("roleId")?.Value;
            if (!int.TryParse(roleIdClaim, out int roleId))
                return Forbid();

            var menuItems = _menuService.GetMenuByRole(roleId);
            return Ok(menuItems);
        }
    }
}

