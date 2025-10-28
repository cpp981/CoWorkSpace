using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using CoWorkSpace.Api.Constants;
using CoWorkSpace.Api.Services;

namespace CoWorkSpace.Api.Controllers
{
    [ApiController]
    [Route("api/v1/roles")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var roles = await _roleService.GetRolesAsync();
                return Ok(roles);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = ApiMessages.INTERNAL_SERVER_ERROR });
            }
        }
    }
}
