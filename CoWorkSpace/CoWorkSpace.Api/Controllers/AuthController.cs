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
    public class AuthController : ControllerBase
    {
        private readonly IRegisterService _registerService;

        public AuthController(IRegisterService registerService)
        {
            _registerService = registerService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            try
            {
                var result = await _registerService.RegisterAsync(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new RegisterResponseDTO { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new RegisterResponseDTO { Message = ApiMessages.INTERNAL_SERVER_ERROR });
            }
        }
    }
}
