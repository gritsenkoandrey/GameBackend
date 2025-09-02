using GameBackend.Scripts.Data;
using GameBackend.Scripts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameBackend.Scripts.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(AppDbContext context, JwtService jwtService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            AuthResult<string> result = new (success: false, error: "Invalid Request");
            
            return BadRequest(result);
        }
        
        User? user = await context.Users.FirstOrDefaultAsync(data => data.Username == request.Username);

        if (user == null)
        {
            user = new()
            {
                Username = request.Username,
            };
            
            string hashPassword = new PasswordHasher<User>().HashPassword(user, request.Password);
            
            user.Password = hashPassword;
            
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            
            string token = jwtService.GenerateToken(user);
            
            AuthResult<string> result = new (success: true, data: token);
            
            return Ok(result);
        }
        else
        {
            AuthResult<string> result = new (success: false, error: "Nickname Is Already In Use");
        
            return Unauthorized(result);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] User request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            AuthResult<string> result = new (success: false, error: "Invalid Request");
            
            return BadRequest(result);
        }
        
        User? user = await context.Users.FirstOrDefaultAsync(data => data.Username == request.Username);

        if (user == null)
        {
            AuthResult<string> result = new (success: false, error: "Invalid Login");

            return Unauthorized(result);
        }
        
        PasswordVerificationResult verificationResult = new PasswordHasher<User>().VerifyHashedPassword(user, user.Password, request.Password);

        if (verificationResult == PasswordVerificationResult.Success)
        {
            string token = jwtService.GenerateToken(user);
                
            AuthResult<string> result = new (success: true, data: token);
            
            return Ok(result);
        }
        else
        {
            AuthResult<string> result = new (success: false, error: "Invalid Password");

            return Unauthorized(result);
        }
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        string userId = User.Claims.First(claim => claim.Type == "id").Value;
        
        int id = int.Parse(userId);
        
        User? user = await context.Users.FindAsync(id);
            
        if (user == null)
        {
            AuthResult<User> result = new (success: false, error: "User Not Found");

            return NotFound(result);
        }
        else
        {
            AuthResult<User> result = new (success: true, data: user);

            return Ok(result);
        }
    }
    
    [Authorize]
    [HttpPost("save")]
    public async Task<IActionResult> SaveProgress([FromBody] User data)
    {
        string userId = User.Claims.First(c => c.Type == "id").Value;
        
        int id = int.Parse(userId);

        User? user = await context.Users.FindAsync(id);
        
        if (user == null)
        {
            AuthResult<User> result = new (success: false, error: "User Not Found");
            
            return NotFound(result);
        }
        else
        {
            user.Level = data.Level;
            user.Money = data.Money;
            
            AuthResult<User> result = new (success: true, data: user);

            await context.SaveChangesAsync();

            return Ok(result);
        }
    }
}