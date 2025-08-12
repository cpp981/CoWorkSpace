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

        [HttpGet]
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
                return NotFound(string.Format(ApiMessages.NoSpacesForProvider, providerId ));

            return Ok(spaces);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSpace(int providerId, [FromBody] SpaceCreateDTO dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var roleId = int.Parse(User.FindFirst("roleId")?.Value ?? "0");

            if (roleId != 3)
            {
                return Forbid(ApiMessages.NoPermissionCreateSpace);
            }
            if (userId != providerId)
            {
                return Forbid(ApiMessages.NoPermissionOtherProvider);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiMessages.InvalidData);
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
                    message = ApiMessages.SpaceCreatedSuccess,
                    spaceId = newSpace.Id
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ApiMessages.SpaceCreatedError
                });
            }
        }


    }
}
