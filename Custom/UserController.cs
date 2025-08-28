using GameBackend.Data;
using Microsoft.AspNetCore.Mvc;

namespace GameBackend.Custom;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }
    
    [HttpPost("login")]
    public IActionResult Login([FromBody] User? loginRequest)
    {
        if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
        {
            return BadRequest("Invalid Request");
        }
        
        User? user = _context.Users.FirstOrDefault(data => data.Username == loginRequest.Username);

        if (user == null)
        {
            user = new()
            {
                Username = loginRequest.Username,
                Password = loginRequest.Password
            };
            
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        else if (user.Password != loginRequest.Password)
        {
            return Unauthorized("Invalid Password");
        }

        return Ok(user);
    }

    [HttpGet("{id}")]
    public IActionResult GetUser(int id)
    {
        User? user = _context.Users.Find(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpPost("{id}/save")]
    public IActionResult SaveProgress(int id, [FromBody] User progress)
    {
        User? user = _context.Users.Find(id);
        if (user == null) return NotFound();

        user.Level = progress.Level;
        user.Money = progress.Money;
        _context.SaveChanges();

        return Ok(user);
    }
}