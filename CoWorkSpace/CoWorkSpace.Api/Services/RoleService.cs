using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CoWorkSpace.Api.Data;

namespace CoWorkSpace.Api.Services
{
    public class RoleService : IRoleService
    {
        private readonly CoWorkSpaceContext _context;

        public RoleService(CoWorkSpaceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<object>> GetRolesAsync()
        {
            var roles = await _context.Roles
                .Where(r => r.RoleId != 2 && r.RoleId != 1) // Excluir Admin y SuperAdmin si corresponde
                .Select(r => new { Id = r.RoleId, Name = r.RoleName })
                .ToListAsync();

            return roles;
        }
    }
}
