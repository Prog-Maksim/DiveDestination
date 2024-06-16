using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DiveDestination.Scripts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Server_Test.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DiveDestination.Controllers;

[ApiController]
[Route("api/registration")]
public class RegistrationController(ILogger<RegistrationController> logger, ApplicationContext context): ControllerBase
{
    private readonly PasswordHasher<Persons> _passwordHasher = new();

    [HttpPost("user")]
    public async Task<IActionResult> RegisterUser([FromForm] string firstName, [FromForm] string lastName, [FromForm] string login, [FromForm] string password)
    {
        var personWithEmail = await context.Persons.SingleOrDefaultAsync(p => p.email == login);
    
        if (CheckData.checkCurrentEmailAddress(login) && CheckData.checkCurrentPassword(password))
        {
            if (personWithEmail != null)
            {
                var problem = new ProblemDetails {
                    Status = 403,
                    Title = "Forbidden",
                    Detail = "данный почтовый адрес уже используется!"
                };

                return Problem(problem.Detail, null, problem.Status, problem.Title);
            }
        
            PassData pass = new PassData();
            UserLevel level = new UserLevel{ level = "пользователь" };

            await context.PassData.AddAsync(pass);
            await context.UserLevel.AddAsync(level);
            await context.SaveChangesAsync();

            Persons user = new Persons
            {
                last_name = firstName,
                first_name = lastName,
                email = login,
                passport = pass.id,
                user_level = level.id,
                image_path = "Images/user-default.png"
            };
            user.password = _passwordHasher.HashPassword(user, password);

            await context.Persons.AddAsync(user);
            await context.SaveChangesAsync();
            
            var claims = new List<Claim> { new(ClaimTypes.Email, login) };
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(30)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var res = new
            {
                message = "Вы успешно создали аккаунт",
                access_token = encodedJwt
            };

            return Ok(res);
        }

        return BadRequest(new { message = "логин или пароль не соответствует требованиям!" });
    }
}