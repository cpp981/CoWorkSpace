using Microsoft.AspNetCore.Mvc;

namespace CoWorkSpace.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "¡Conexión exitosa con el backend!" });
        }
    }
}
