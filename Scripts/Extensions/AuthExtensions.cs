using System.Text;
using GameBackend.Scripts.Controllers;
using GameBackend.Scripts.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace GameBackend.Scripts.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddAuth(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        AuthSettings? authSettings = configuration.GetSection(nameof(AuthSettings)).Get<AuthSettings>();
        
        serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(param =>
        {
            param.TokenValidationParameters = new ()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.SecretKey)),
            };
            
            param.Events = new ()
            {
                OnChallenge = async context =>
                {
                    context.HandleResponse();

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    AuthResult<string> result = new (false, string.Empty, "Token Expired or Invalid");
                    await context.Response.WriteAsJsonAsync(result);
                }
            };
        });
        
        return serviceCollection;
    }
}