using System.Threading.Tasks;
using CoWorkSpace.Api.DTOs;

namespace CoWorkSpace.Api.Services
{
    public interface IRegisterService
    {
        
        // Registra un nuevo usuario. Lanza ArgumentException si los datos son inválidos.
        
        Task<RegisterResponseDTO> RegisterAsync(RegisterRequestDTO request);
    }
}

