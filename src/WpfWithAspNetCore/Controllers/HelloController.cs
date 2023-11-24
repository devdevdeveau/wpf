using Microsoft.AspNetCore.Mvc;

namespace WpfWithAspNetCore.Controllers;

[ApiController]
[Route("[controller]")]
public class HelloController : ControllerBase
{
    [HttpGet("{name}")]
    public IActionResult Get(string name)
    {
        return Ok($"Hello {name}");
    }
}