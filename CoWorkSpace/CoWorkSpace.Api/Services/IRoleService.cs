using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoWorkSpace.Api.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<object>> GetRolesAsync();
    }
}

