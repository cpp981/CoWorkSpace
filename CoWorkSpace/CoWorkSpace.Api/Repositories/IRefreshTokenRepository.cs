using CoWorkSpace.Api.Models;

namespace CoWorkSpace.Api.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token);
        Task<RefreshToken?> GetByHashAsync(string hash);
        Task RevokeAsync(RefreshToken token, string? replacedByHash = null);
        Task<IEnumerable<RefreshToken>> GetActiveByUserAsync(int userId);
    }

}
