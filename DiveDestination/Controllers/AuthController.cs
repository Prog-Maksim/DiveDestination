using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;


namespace DiveDestination.Controllers;

[ApiController]
[Route("api/authorization")]
public class AuthController: ControllerBase
{
    [HttpPost("refresh-token")]
    public IActionResult RefreshToken([FromForm] string access_token)
    {
        try
        {
            var principal = GetPrincipalFromExpiredToken(access_token);
            if (principal == null)
            {
                return BadRequest("Invalid token");
            }

            var newJwtToken = GenerateNewToken(principal);
            return Ok(new
            {
                message = "токен успешно обновлен",
                access_token = newJwtToken
            });
        }
        catch
        {
            return BadRequest("Invalid token");
        }
    }
    
    private string GenerateNewToken(ClaimsPrincipal principal)
    {
        var claims = principal.Claims.ToList();
        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(5)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
    
    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = AuthOptions.ISSUER,
            ValidAudience = AuthOptions.AUDIENCE,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey()
        };

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
        catch (ArgumentException)
        {
            throw new SecurityTokenException("Invalid token");
        }
    }
}