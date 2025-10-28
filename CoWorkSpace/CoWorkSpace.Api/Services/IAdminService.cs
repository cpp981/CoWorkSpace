using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CoWorkSpace.Api.DTOs;

namespace CoWorkSpace.Api.Services
{
    public interface IAdminService
    {
        Task<List<AdminSpacesResponseDTO>> GetAdminSpacesAsync(int adminId, ClaimsPrincipal user);
        Task<List<BookingForAdminDTO>> GetSpaceBookingsAsync(int adminId, int spaceId, ClaimsPrincipal user);
        Task<List<ClientWithSpacesDTO>> GetClientsForAdminAsync(int adminId, ClaimsPrincipal user);
        Task<BookingUpdateResultDTO> UpdateBookingAsync(int adminId, int spaceId, int bookingId, UpdateBookingDTO dto, ClaimsPrincipal user);
        Task DeleteBookingAsync(int adminId, int spaceId, int bookingId, ClaimsPrincipal user);
    }
}
