using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CoWorkSpace.Api.DTOs;

namespace CoWorkSpace.Api.Services
{
    public interface IProviderService
    {
        Task<List<AdminInfoDTO>> GetAdminsForProviderAsync(ClaimsPrincipal user);
        Task<RegisterResponseDTO> CreateAdminAsync(int providerId, RegisterRequestDTO request, ClaimsPrincipal user);
        Task<UpdateAdminResponseDTO> UpdateAdminAsync(int providerId, int adminId, UpdateAdminRequestDTO request, ClaimsPrincipal user);
        Task<DeleteAdminResponseDto> DeleteAdminAsync(int providerId, int adminId, ClaimsPrincipal user);
    }
}

