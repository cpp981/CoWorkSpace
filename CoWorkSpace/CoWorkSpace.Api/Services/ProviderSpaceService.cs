using CoWorkSpace.Api.Constants;
using CoWorkSpace.Api.Data;
using CoWorkSpace.Api.DTOs;
using CoWorkSpace.Api.DTOs.Provider;
using CoWorkSpace.Api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoWorkSpace.Api.Services
{
    public class ProviderSpaceService : IProviderSpaceService
    {
        private readonly CoWorkSpaceContext _context;

        public ProviderSpaceService(CoWorkSpaceContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        private (int roleId, int userId) ExtractClaims(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleIdClaim = user.FindFirst("roleId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(roleIdClaim)
                || !int.TryParse(userIdClaim, out int userId)
                || !int.TryParse(roleIdClaim, out int roleId))
            {
                throw new UnauthorizedAccessException(ApiMessages.UNAUTHORIZED);
            }

            return (roleId, userId);
        }

        public async Task<List<ProviderSpaceDTO>> GetSpacesByProviderAsync(int providerId, ClaimsPrincipal user)
        {
            var (roleId, userId) = ExtractClaims(user);

            if (roleId != 3) throw new UnauthorizedAccessException(ApiMessages.NO_PERMISSION_UPDATE_SPACE);
            if (userId != providerId) throw new UnauthorizedAccessException(ApiMessages.NO_PERMISSION_UPDATE_OTHER_PROVIDER);

            var spaces = await _context.Spaces
                .Where(s => s.ProviderId == providerId && !s.IsDeleted)
                .Select(s => new ProviderSpaceDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    AdminId = s.AdminId,
                    IsPublic = s.IsPublic,
                    Price = s.Price,
                    City = s.City,
                    IsActive = !s.IsDeleted
                })
                .ToListAsync();

            if (spaces.Count == 0)
                throw new KeyNotFoundException(string.Format(ApiMessages.NO_SPACES_FOR_PROVIDER, providerId));

            return spaces;
        }

        public async Task<int> EditSpaceAsync(int providerId, int id, SpaceCreateDTO dto, ClaimsPrincipal user)
        {
            var (roleId, userId) = ExtractClaims(user);

            if (roleId != 3) throw new UnauthorizedAccessException(ApiMessages.NO_PERMISSION_UPDATE_SPACE);
            if (userId != providerId) throw new UnauthorizedAccessException(ApiMessages.NO_PERMISSION_UPDATE_OTHER_PROVIDER);

            if (dto == null)
                throw new ArgumentException(ApiMessages.INVALID_DATA);

            var space = await _context.Spaces
                .FirstOrDefaultAsync(s => s.Id == id && s.ProviderId == providerId && !s.IsDeleted);

            if (space == null) throw new KeyNotFoundException(ApiMessages.SPACE_NOT_FOUND);

            space.Name = dto.Name;
            space.AdminId = dto.AdminId;
            space.IsPublic = dto.IsPublic;
            space.Price = dto.Price;
            space.City = dto.City;

            await _context.SaveChangesAsync();

            return space.Id;
        }

        public async Task DeleteSpaceAsync(int providerId, int id, ClaimsPrincipal user)
        {
            var (roleId, userId) = ExtractClaims(user);

            if (roleId != 3) throw new UnauthorizedAccessException(ApiMessages.NO_PERMISSION_DELETE_SPACE);
            if (userId != providerId) throw new UnauthorizedAccessException(ApiMessages.NO_PERMISSION_DELETE_OTHER_PROVIDER_SPACES);

            var space = await _context.Spaces
                .FirstOrDefaultAsync(s => s.Id == id && s.ProviderId == providerId);

            if (space == null) throw new KeyNotFoundException(ApiMessages.SPACE_NOT_FOUND);

            space.IsDeleted = true;
            _context.Spaces.Update(space);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CreateSpaceAsync(int providerId, SpaceCreateDTO dto, ClaimsPrincipal user)
        {
            var (roleId, userId) = ExtractClaims(user);

            if (roleId != 3) throw new UnauthorizedAccessException(ApiMessages.INVALID_DATA);
            if (userId != providerId) throw new UnauthorizedAccessException(ApiMessages.NO_PERMISSION_OTHER_PROVIDER);

            if (dto == null)
                throw new ArgumentException(ApiMessages.INVALID_DATA);

            var newSpace = new Space
            {
                Name = dto.Name,
                AdminId = dto.AdminId,
                IsPublic = dto.IsPublic,
                Price = dto.Price,
                City = dto.City,
                ProviderId = providerId,
                IsDeleted = false
            };

            _context.Spaces.Add(newSpace);
            await _context.SaveChangesAsync();

            return newSpace.Id;
        }
    }
}
