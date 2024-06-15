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
public class RegistrationController: ControllerBase
{
    private readonly ILogger<RegistrationController> _logger;
    private readonly ApplicationContext _context;
    private readonly PasswordHasher<Persons> _passwordHasher;

    public RegistrationController(ILogger<RegistrationController> logger, ApplicationContext context)
    {
        _logger = logger;
        _context = context;
        _passwordHasher = new PasswordHasher<Persons>();
    }

    [HttpPost("user")]
    public async Task<IActionResult> RegisterUser([FromForm] string first_name, [FromForm] string last_name, [FromForm] string login, [FromForm] string password)
    {
        _logger.LogInformation($"Вызван эндпоинт для регистрации \n----------\nпереданные данные: \nимя: {first_name} \nлогин: {login} \nпароль: {password}");
    
        List<Persons> person = await _context.Persons.ToListAsync();

        bool dubl_email = !person.GroupBy(p => p.email).Any(g => g.Count() > 1);
    
        if (CheckData.checkCurrentEmailAddress(login) && CheckData.checkCurrentPassword(password))
        {
            if (dubl_email)
            {
                var problem = new ProblemDetails {
                    Status = 403,
                    Title = "Forbidden",
                    Detail = "данный почтовый адрес уже используется!"
                };

                return Problem(problem.Detail, null, problem.Status, problem.Title);
            }
        
            PassData pass = new PassData {
                pass_num = 1234,
                pass_seria = 567890,
                pass_issued = "Гу МВД РОССИИ ПО РО",
                pass_date_start = DateTime.Now
            };
            UserLevel level = new UserLevel
            {
                level = "пользователь"
            };
        
            await _context.PassData.AddAsync(pass);
            await _context.UserLevel.AddAsync(level);
        
            Persons user = new Persons {
                last_name = first_name,
                first_name = last_name,
                patronymic = "",
                age = 18,
                email = login,
                passport = 1,
                image_path = "Images/user-default.png",
                user_level = 1
            };
            user.password = _passwordHasher.HashPassword(user, password);
        
            await _context.Persons.AddAsync(user);
            await _context.SaveChangesAsync();
        
            var claims = new List<Claim> { new(ClaimTypes.Name, login) };

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
                access_token = encodedJwt,
                email = login
            };

            return Ok(res);
        }

        return BadRequest(new { message = "логин или пароль не соответствует требованиям!" });
    }
}