using Microsoft.AspNetCore.Mvc;
using CoWorkSpace.Api.Data;
using CoWorkSpace.Api.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

namespace CoWorkSpace.Api.Controllers
{
    [ApiController]
    [Route("api/auth/login")]
    public class LoginController : ControllerBase
    {
        private readonly CoWorkSpaceContext _context;
        private readonly IConfiguration _configuration;

        public LoginController(CoWorkSpaceContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                return BadRequest("Email y contraseña son requeridos.");

            // Incluyo el Role para poder acceder a su nombre
            var user = await _context.Users
                .IgnoreQueryFilters()
                .Include(u => u.Role)  // <-- Aquí para traer el Role
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null)
                return Unauthorized("Credenciales inválidas.");

            // Validar contraseña con BCrypt
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            if (!isPasswordValid)
                return Unauthorized("Credenciales inválidas.");

            // Uso el nombre del rol en el claim
            var roleName = user.Role?.RoleName ?? "User"; // Si por alguna razón no tiene rol, asigno "User"

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("roleId", user.RoleId.ToString()),  // Guardamos roleId como claim personalizado
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddHours(2);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var response = new LoginResponseDto
            {
                Token = tokenString,
                Expiration = expiration
            };

            return Ok(response);
        }
    }
}

