using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DiveDestination.Scripts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DiveDestination.Controllers;

[ApiController]
[Route("api/registration")]
public class RegistrationController: ControllerBase
{
    List<PersonData> users = new List<PersonData> { 
        new() { Id = "usr_1", loginEmail = "Grigorieva@gmail.com",loginNumber = "81234567890", Name = "Григорьева", Age = 37, password = "asdgdhjt8@u4y5ved"},
        new() { Id = "usr_2", loginEmail = "matvey_frontender@gmail.com", Name = "Матвей фронтендер", Age = 10, password = "asfaer67u5nktu("},
        new() { Id = "usr_3", loginEmail = "maksim_bekender@gmail.com", loginNumber = "80987654321", Name = "Максим Бекендер", Age = 24, password = "b9s9573@(74tnv"}
    };
    
    private readonly ILogger<RegistrationController> _logger;
    private readonly ApplicationContext _context;

    public RegistrationController(ILogger<RegistrationController> logger, ApplicationContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpPost("user")]
    public IActionResult RegisterUser([FromForm] string name, [FromForm] string login, [FromForm] string password)
    {
        _logger.LogInformation($"Вызван эндпоинт для регистрации \n----------\nпереданные данные: \nимя: {name} \nлогин: {login} \nпароль: {password}");
    
        if (!Request.Cookies.ContainsKey("personId"))
        {
            Random rnd = new Random();
            PersonData data = new PersonData();
            int num = rnd.Next(11111, 99999);
            data.Id = $"usr_{num}";
            data.Name = name;
            data.password = password;
        
            if (CheckData.checkCurrentEmailAddress(login) && CheckData.checkCurrentPassword(password))
            {
                if (CheckData.checkDublicatePersonDataEmailAddress(users, login))
                {
                    var problem = new ProblemDetails {
                        Status = 403,
                        Title = "Forbidden",
                        Detail = "данный почтовый адрес уже используется!"
                    };

                    return Problem(problem.Detail, null, problem.Status, problem.Title);
                }
            
                data.loginEmail = login;
                users.Add(data);
            
                var claims = new List<Claim> {new Claim(ClaimTypes.Name, login) };
                // создаем JWT-токен
                var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(30)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
 
                // формируем ответ
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

        return Ok(new { message = "вы уже авторизованы!" });
    }
}