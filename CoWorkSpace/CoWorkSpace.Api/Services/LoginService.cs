using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using CoWorkSpace.Api.Data;
using CoWorkSpace.Api.DTOs;
using CoWorkSpace.Api.Constants;
using CoWorkSpace.Api.Models;
using CoWorkSpace.Api.Repositories;

namespace CoWorkSpace.Api.Services
{
    public class LoginService : ILoginService
    {
        private readonly CoWorkSpaceContext _context;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginService(
            CoWorkSpaceContext context,
            ITokenService tokenService,
            IRefreshTokenRepository refreshRepo,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _tokenService = tokenService;
            _refreshRepo = refreshRepo;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                throw new ArgumentException(ApiMessages.MAIL_AND_PASSWORD_ARE_REQUIRED);

            var user = await _context.Users
                .IgnoreQueryFilters()
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null)
                throw new UnauthorizedAccessException(ApiMessages.INVALID_CREDENTIALS);

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            if (!isPasswordValid)
                throw new UnauthorizedAccessException(ApiMessages.INVALID_CREDENTIALS);

            // Crear access token
            var accessToken = _tokenService.CreateAccessToken(user, out var accessExpiration);

            // Crear refresh token (plano + hash)
            var (refreshPlain, refreshHash) = _tokenService.GenerateRefreshToken();

            var refresh = new RefreshToken
            {
                TokenHash = refreshHash,
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(30),
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString()
            };

            await _refreshRepo.AddAsync(refresh);

            // Setear cookie HttpOnly
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = refresh.Expires
            };
            _httpContextAccessor.HttpContext?.Response.Cookies.Append("refreshToken", refreshPlain, cookieOptions);

            return new LoginResponseDto
            {
                Token = accessToken,
                Expiration = accessExpiration,
                Message = ApiMessages.LOGIN_SUCCESS
            };
        }
    }
}
