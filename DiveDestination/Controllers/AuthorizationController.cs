using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server_Test.Models;

namespace DiveDestination.Controllers;

[ApiController]
[Route("api/authorization")]
public class AuthorizationController(ILogger<AuthorizationController> logger, ApplicationContext context): ControllerBase
{
    private readonly PasswordHasher<Persons> _passwordHasher = new();

    [HttpPost("user")]
    public async Task<IActionResult> AuthorizeUser([FromForm] string login, [FromForm] string password)
    {
        logger.LogInformation($"Вызван эндпоинт для авторизации\n----------\nпереданные данные: \nлогин: {login} \nпароль:{password}");

        var person = await context.Persons.FirstOrDefaultAsync(p => p.email == login);

        if (person != null)
        {
            if (person.status == false)
            {
                var problems = new ProblemDetails {
                    Status = 403,
                    Title = "Forbidden",
                    Detail = "Данный пользователь заблокирован!"
                };

                return Problem(problems.Detail, null, problems.Status, problems.Title);
            }
            
            var result = _passwordHasher.VerifyHashedPassword(person, person.password, password);

            if (result == PasswordVerificationResult.Success)
            {
                var claims = new List<Claim> { new(ClaimTypes.Email, person.email) };
            
                var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(5)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
        
                var res = new
                {
                    message = "вы успешно авторизовались",
                    access_token = encodedJwt
                };

                return Ok(res);
            }
            var problemDetails = new ProblemDetails {
                Status = 403,
                Title = "Forbidden",
                Detail = "Логин или пароль не верен!"
            };

            return Problem(problemDetails.Detail, null, problemDetails.Status, problemDetails.Title);
        }
        var problem = new ProblemDetails {
            Status = 403,
            Title = "Forbidden",
            Detail = "Логин или пароль введен не корректно!"
        };

        return Problem(problem.Detail, null, problem.Status, problem.Title);
    }
    
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