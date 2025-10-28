using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CoWorkSpace.Api.Data;
using CoWorkSpace.Api.DTOs;
using CoWorkSpace.Api.Constants;
using CoWorkSpace.Api.Models;

namespace CoWorkSpace.Api.Services
{
    public class ProviderService : IProviderService
    {
        private readonly CoWorkSpaceContext _context;

        public ProviderService(CoWorkSpaceContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        private (int roleId, int userId) ExtractClaims(ClaimsPrincipal user)
        {
            var roleClaim = user.FindFirst("roleId")?.Value;
            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(roleClaim) || string.IsNullOrEmpty(idClaim)
                || !int.TryParse(roleClaim, out int roleId)
                || !int.TryParse(idClaim, out int userId))
            {
                throw new UnauthorizedAccessException(ApiMessages.UNAUTHORIZED);
            }

            return (roleId, userId);
        }

        public async Task<List<AdminInfoDTO>> GetAdminsForProviderAsync(ClaimsPrincipal user)
        {
            var (roleId, loggedUserId) = ExtractClaims(user);

            if (roleId != 3) // provider
                throw new UnauthorizedAccessException(ApiMessages.ONLY_PROVIDERS_CAN_VIEW_ADMINS);

            var admins = await _context.Users
                .Where(u => u.RoleId == 2 && u.ProviderId == loggedUserId)
                .Select(u => new AdminInfoDTO { Id = u.Id, Name = u.Name, Email = u.Email })
                .ToListAsync();

            if (!admins.Any())
                throw new KeyNotFoundException(ApiMessages.NO_ADMINS_FOUND);

            return admins;
        }

        public async Task<RegisterResponseDTO> CreateAdminAsync(int providerId, RegisterRequestDTO request, ClaimsPrincipal user)
        {
            if (request == null)
                throw new ArgumentException(ApiMessages.INVALID_DATA);

            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
                throw new ArgumentException(ApiMessages.PASSWORD_TOO_SHORT);

            var (roleId, userId) = ExtractClaims(user);

            if (roleId != 3)
                throw new UnauthorizedAccessException(ApiMessages.ONLY_PROVIDERS_CAN_CREATE_ADMINS);

            if (userId != providerId)
                throw new UnauthorizedAccessException(ApiMessages.CANNOT_CREATE_ADMINS_FOR_OTHER_PROVIDERS);

            if (request.RoleId != 2)
                throw new ArgumentException(ApiMessages.ONLY_ADMIN_ROLE_ALLOWED);

            var exists = await _context.Users.IgnoreQueryFilters().AnyAsync(u => u.Email == request.Email);
            if (exists)
                throw new ArgumentException(ApiMessages.EMAIL_ALREADY_REGISTERED);

            var adminUser = new User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Name = request.Name,
                RoleId = 2,
                ProviderId = providerId
            };

            _context.Users.Add(adminUser);
            await _context.SaveChangesAsync();

            return new RegisterResponseDTO
            {
                Id = adminUser.Id,
                Email = adminUser.Email,
                Name = adminUser.Name,
                RoleId = adminUser.RoleId,
                ProviderId = adminUser.ProviderId,
                Message = ApiMessages.ADMIN_CREATED_SUCCESS
            };
        }

        public async Task<UpdateAdminResponseDTO> UpdateAdminAsync(int providerId, int adminId, UpdateAdminRequestDTO request, ClaimsPrincipal user)
        {
            var (roleId, loggedUserId) = ExtractClaims(user);

            if (roleId != 3)
                throw new UnauthorizedAccessException(ApiMessages.ONLY_PROVIDERS_CAN_EDIT_ADMINS);

            if (loggedUserId != providerId)
                throw new UnauthorizedAccessException(ApiMessages.CANNOT_EDIT_ADMINS_FOR_OTHER_PROVIDERS);

            var adminUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == adminId && u.ProviderId == providerId && u.RoleId == 2);

            if (adminUser == null)
                throw new KeyNotFoundException(ApiMessages.ADMIN_NOT_FOUND);

            if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != adminUser.Email)
            {
                var exists = await _context.Users.AnyAsync(u => u.Email == request.Email);
                if (exists)
                    throw new ArgumentException(ApiMessages.EMAIL_ALREADY_REGISTERED);

                adminUser.Email = request.Email;
            }

            adminUser.Name = request.Name ?? adminUser.Name;

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                if (request.Password.Length < 6)
                    throw new ArgumentException(ApiMessages.PASSWORD_TOO_SHORT);

                adminUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            await _context.SaveChangesAsync();

            return new UpdateAdminResponseDTO
            {
                Id = adminUser.Id,
                Name = adminUser.Name,
                Email = adminUser.Email,
                ProviderId = (int)adminUser.ProviderId,
                RoleId = adminUser.RoleId,
                Message = ApiMessages.ADMIN_UPDATED_SUCCESS
            };
        }

        public async Task<DeleteAdminResponseDto> DeleteAdminAsync(int providerId, int adminId, ClaimsPrincipal user)
        {
            var (roleId, loggedUserId) = ExtractClaims(user);

            if (roleId != 3)
                throw new UnauthorizedAccessException(ApiMessages.UNAUTHORIZED);

            if (loggedUserId != providerId)
                throw new UnauthorizedAccessException(ApiMessages.CANNOT_DELETE_ADMINS_FOR_OTHER_PROVIDERS);

            var adminUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == adminId && u.ProviderId == providerId && u.RoleId == 2);

            if (adminUser == null)
                throw new KeyNotFoundException(ApiMessages.ADMIN_NOT_FOUND);

            _context.Users.Remove(adminUser);
            await _context.SaveChangesAsync();

            return new DeleteAdminResponseDto
            {
                Id = adminUser.Id,
                Name = adminUser.Name,
                Email = adminUser.Email,
                Message = ApiMessages.ADMIN_DELETED_SUCCESS
            };
        }
    }
}

