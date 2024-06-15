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
public class AuthorizationController: ControllerBase
{
    private readonly ILogger<AuthorizationController> _logger;
    private readonly ApplicationContext _context;
    private readonly PasswordHasher<Persons> _passwordHasher;

    public AuthorizationController(ILogger<AuthorizationController> logger, ApplicationContext context)
    {
        _logger = logger;
        _context = context;
        _passwordHasher = new PasswordHasher<Persons>();
    }

    [HttpPost("user")]
    public async Task<IActionResult> AuthorizeUser([FromForm] string login, [FromForm] string password)
    {
        _logger.LogInformation($"Вызван эндпоинт для авторизации\n----------\nпереданные данные: \nлогин: {login} \nпароль:{password}");

        var person = await _context.Persons.FirstOrDefaultAsync(p => p.email == login);

        if (person != null)
        {
            var result = _passwordHasher.VerifyHashedPassword(person, person.password, password);

            if (result == PasswordVerificationResult.Success)
            {
                var claims = new List<Claim> { new(ClaimTypes.Name, person.first_name), new(ClaimTypes.Email, person.email) };
            
                var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(30)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
        
                var res = new
                {
                    message = "вы успешно авторизовались",
                    access_token = encodedJwt,
                    email = person.email
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
}