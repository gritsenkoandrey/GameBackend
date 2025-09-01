using GameBackend.Scripts.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameBackend.Scripts.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(AppDbContext context) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] User? request)
    {
        if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest("Invalid Request");
        }
        
        User? user = await context.Users.FirstOrDefaultAsync(data => data.Username == request.Username);

        if (user == null)
        {
            user = new()
            {
                Username = request.Username,
                Password = request.Password
            };
            
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }
        else if (user.Password != request.Password)
        {
            return Unauthorized("Invalid Password");
        }

        return Ok(user);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUser(int id)
    {
        User? user = await context.Users.FindAsync(id);
        
        if (user == null)
        {
            return NotFound();
        }
        
        return Ok(user);
    }

    [HttpPost("{id:int}/save")]
    public async Task<IActionResult> SaveProgress(int id, [FromBody] User data)
    {
        User? user = await context.Users.FindAsync(id);
        
        if (user == null)
        {
            return NotFound();
        }

        user.Level = data.Level;
        user.Money = data.Money;
        
        await context.SaveChangesAsync();

        return Ok(user);
    }
}