using CoWorkSpace.Api.DTOs;
using CoWorkSpace.Api.DTOs.Provider;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoWorkSpace.Api.Services
{
    public interface IProviderSpaceService
    {
        Task<List<ProviderSpaceDTO>> GetSpacesByProviderAsync(int providerId, ClaimsPrincipal user);
        Task<int> EditSpaceAsync(int providerId, int id, SpaceCreateDTO dto, ClaimsPrincipal user);
        Task DeleteSpaceAsync(int providerId, int id, ClaimsPrincipal user);
        Task<int> CreateSpaceAsync(int providerId, SpaceCreateDTO dto, ClaimsPrincipal user);
    }
}

