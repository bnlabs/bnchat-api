using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ToffApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HealthController : Controller
{
    public HealthController()
    {

    }
    
    [HttpGet("/getVersion")]
    public IActionResult Version()
    {
        const string filePath = "buildinfo.json";
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound();
        }
        
        string jsonString = System.IO.File.ReadAllText(filePath);
        var jsonData = JsonSerializer.Deserialize<Dictionary<string, BuildInfo>>(jsonString);

        var buildInfo = jsonData["buildInfo"];

        return Ok(buildInfo);
    }
}