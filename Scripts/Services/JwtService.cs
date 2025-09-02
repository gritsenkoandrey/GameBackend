using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GameBackend.Scripts.Data;
using GameBackend.Scripts.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GameBackend.Scripts.Services;

public sealed class JwtService(IOptions<AuthSettings>  settings)
{
    public string GenerateToken(User user)
    {
        DateTime expires = DateTime.UtcNow.Add(settings.Value.Expires);
        
        List<Claim> claims =
        [
            new ("id", user.Id.ToString()),
        ];
        
        SigningCredentials signing = new (new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Value.SecretKey)), SecurityAlgorithms.HmacSha256);
        
        JwtSecurityToken token = new (expires: expires, claims: claims, signingCredentials: signing);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}