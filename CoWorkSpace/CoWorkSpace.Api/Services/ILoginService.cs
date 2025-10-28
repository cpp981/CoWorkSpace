using CoWorkSpace.Api.DTOs;

namespace CoWorkSpace.Api.Services
{
    public interface ILoginService
    {
        
        /// Valida credenciales, crea access token y refresh token, guarda el refresh token
        /// y añade la cookie al HttpResponse.
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginDto);
    }
}
