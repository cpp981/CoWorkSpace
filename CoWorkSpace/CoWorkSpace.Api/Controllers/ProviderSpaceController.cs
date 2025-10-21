using CoWorkSpace.Api.Constants;
using CoWorkSpace.Api.Data;
using CoWorkSpace.Api.DTOs;
using CoWorkSpace.Api.DTOs.Provider;
using CoWorkSpace.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CoWorkSpace.Api.Controllers
{
    [ApiController]
    [Route("api/v1/providers/{providerId}/spaces")]
    [Authorize]
    public class ProviderSpacesController : ControllerBase
    {
        private readonly CoWorkSpaceContext _context;

        public ProviderSpacesController(CoWorkSpaceContext context)
        {
            _context = context;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetSpacesByProvider(int providerId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var roleId = int.Parse(User.FindFirst("roleId")?.Value ?? "0");

            if (roleId != 3) // Provider RoleId
            {
                return Forbid();
            }
            if (userId != providerId)
            {
                return Forbid();
            }
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
                return NotFound(string.Format(ApiMessages.NO_SPACES_FOR_PROVIDER, providerId));

            return Ok(spaces);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditSpace(int providerId, int id, [FromBody] SpaceCreateDTO dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var roleId = int.Parse(User.FindFirst("roleId")?.Value ?? "0");

            if (roleId != 3)
            {
                return StatusCode(403, new
                {
                    success = false,
                    message = ApiMessages.NO_PERMISSION_UPDATE_SPACE
                });
            }

            if (userId != providerId)
            {
                return StatusCode(403, new
                {
                    success = false,
                    message = ApiMessages.NO_PERMISSION_UPDATE_OTHER_PROVIDER
                });
            }

            if (!ModelState.IsValid)
            {
                return StatusCode(403, new
                {
                    success = false,
                    message = ApiMessages.INVALID_DATA
                });
            }

            try
            {
                var space = await _context.Spaces
                    .FirstOrDefaultAsync(s => s.Id == id && s.ProviderId == providerId && !s.IsDeleted);

                if (space == null)
                    return NotFound(new { success = false, message = ApiMessages.SPACE_NOT_FOUND });

                space.Name = dto.Name;
                space.AdminId = dto.AdminId;
                space.IsPublic = dto.IsPublic;
                space.Price = dto.Price;
                space.City = dto.City;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = ApiMessages.SPACE_UPDATED_SUCCESS,
                    id = space.Id
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ApiMessages.SPACE_UPDATED_ERROR
                });
            }
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteSpace(int providerId, int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var roleId = int.Parse(User.FindFirst("roleId")?.Value ?? "0");

            if (roleId != 3)
            {
                return StatusCode(403, new
                {
                    success = false,
                    message = ApiMessages.NO_PERMISSION_DELETE_SPACE
                });
            }
            if(userId != providerId)
            {
                return StatusCode(403, new
                {
                    success = false,
                    message = ApiMessages.NO_PERMISSION_DELETE_OTHER_PROVIDER_SPACES
                });
                  
            }

            try
            {
                // Buscar el espacio por Id y ProviderId
                var space = await _context.Spaces
                    .FirstOrDefaultAsync(s => s.Id == id && s.ProviderId == providerId);

                if (space == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = ApiMessages.SPACE_NOT_FOUND
                    });
                }

                // Marcar como eliminado
                space.IsDeleted = true;

                _context.Spaces.Update(space);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = ApiMessages.SPACE_DELETED_SUCCESS
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ApiMessages.SPACE_DELETED_ERROR
                });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSpace(int providerId, [FromBody] SpaceCreateDTO dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var roleId = int.Parse(User.FindFirst("roleId")?.Value ?? "0");

            if (roleId != 3)
            {
                return StatusCode(403, new
                {
                    success = false,
                    message = ApiMessages.INVALID_DATA
                });
            }
            if (userId != providerId)
            {
                return StatusCode(403, new
                {
                    success = false,
                    message = ApiMessages.NO_PERMISSION_OTHER_PROVIDER
                });
            }

            if (!ModelState.IsValid)
            {
                return StatusCode(403, new
                {
                    success = false,
                    message = ApiMessages.INVALID_DATA
                });
            }

            try
            {
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

                return Ok(new
                {
                    success = true,
                    message = ApiMessages.SPACE_CREATED_SUCCESS,
                    spaceId = newSpace.Id
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ApiMessages.SPACE_CREATED_ERROR
                });
            }
        }
    }
}
