using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CoWorkSpace.Api.Data;
using CoWorkSpace.Api.DTOs;
using CoWorkSpace.Api.Constants;
using CoWorkSpace.Api.Models;

namespace CoWorkSpace.Api.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly CoWorkSpaceContext _context;

        public RegisterService(CoWorkSpaceContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<RegisterResponseDTO> RegisterAsync(RegisterRequestDTO request)
        {
            if (request == null)
                throw new ArgumentException(ApiMessages.INVALID_DATA);

            // Permitir solo roles 3 y 4 para registrar
            if (request.RoleId != 3 && request.RoleId != 4)
                throw new ArgumentException(ApiMessages.ROLE_NOT_ALLOWED_ONLY_CAN_REGISTER_PROVIDER_OR_CLIENT);

            // Verificar si el RoleId solicitado existe
            var roleEntity = await _context.Roles.FindAsync(request.RoleId);
            if (roleEntity == null)
                throw new ArgumentException(ApiMessages.INVALID_ROLE_OR_ROLEID_NOT_FOUND);

            // Verificar si el email ya existe
            var existingUser = await _context.Users.IgnoreQueryFilters().AnyAsync(u => u.Email == request.Email);
            if (existingUser)
                throw new ArgumentException(ApiMessages.EMAIL_ALREADY_REGISTERED);

            var user = new User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Name = request.Name,
                RoleId = request.RoleId,
                ProviderId = null
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new RegisterResponseDTO
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                RoleId = user.RoleId,
                ProviderId = user.ProviderId,
                Message = ApiMessages.USER_REGISTERED_SUCCESS
            };
        }
    }
}
