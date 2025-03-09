using Microsoft.AspNetCore.Mvc;

namespace hello_world_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HelloController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { message = "Hello forld" });
    }

    [HttpGet("newworld")]
    public string NewWorld()
    {
        return "new world";
    }
}

[ApiController]
[Route("api/[controller]")]
public class MelloController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { message = "H" });
    }
}
