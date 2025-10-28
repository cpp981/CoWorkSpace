using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CoWorkSpace.Api.DTOs;

namespace CoWorkSpace.Api.Services
{
    public interface ISpaceBookingService
    {
        Task<List<BookingResponseDTO>> GetBookingsForSpaceAsync(int spaceId, ClaimsPrincipal user);
    }
}

