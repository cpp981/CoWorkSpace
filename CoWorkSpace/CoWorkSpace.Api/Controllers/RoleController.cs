using Microsoft.AspNetCore.Mvc;
using CoWorkSpace.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace CoWorkSpace.Api.Controllers
{
    [ApiController]
    [Route("api/v1/roles")]
    public class RoleController : ControllerBase
    {
        private readonly CoWorkSpaceContext _context;

        public RoleController(CoWorkSpaceContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _context.Roles
                .Where(r => r.RoleId != 2 && r.RoleId != 1) // Excluir rol Admin
                .Select(r => new { Id = r.RoleId, Name = r.RoleName })
                .ToListAsync();

            return Ok(roles);
        }
    }
}
