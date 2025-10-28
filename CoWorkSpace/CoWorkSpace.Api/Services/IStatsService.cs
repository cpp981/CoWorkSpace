using System.Security.Claims;
using System.Threading.Tasks;
using CoWorkSpace.Api.DTOs;

namespace CoWorkSpace.Api.Services
{
    public interface IStatsService
    {
        Task<SuperAdminStatsDTO> GetSuperAdminStatsAsync(int idUser, ClaimsPrincipal user);
        Task<AdminStatsDTO> GetAdminStatsAsync(int idUser, ClaimsPrincipal user);
        Task<ProviderStatsDTO> GetProviderStatsAsync(int idUser, ClaimsPrincipal user);
        Task<ClientStatsDTO> GetClientStatsAsync(int idUser, ClaimsPrincipal user);
    }
}
