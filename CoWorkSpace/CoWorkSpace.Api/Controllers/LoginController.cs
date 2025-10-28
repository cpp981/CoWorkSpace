using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using CoWorkSpace.Api.DTOs;
using CoWorkSpace.Api.Constants;
using CoWorkSpace.Api.Services;

namespace CoWorkSpace.Api.Controllers
{
    [ApiController]
    [Route("api/v1/")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            try
            {
                var result = await _loginService.LoginAsync(loginDto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new LoginResponseDto { Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new LoginResponseDto { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new LoginResponseDto { Message = ApiMessages.INTERNAL_SERVER_ERROR });
            }
        }
    }
}
