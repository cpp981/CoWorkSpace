using CoWorkSpace.Api.Models;

namespace CoWorkSpace.Api.Services
{
    public interface ITokenService
    {
        string CreateAccessToken(User user, out DateTime expiresAt);
        (string Plain, string Hash) GenerateRefreshToken();
        string HashToken(string token);
    }
}
