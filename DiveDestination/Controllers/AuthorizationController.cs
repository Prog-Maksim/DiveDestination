using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DiveDestination.Scripts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Server_Test.Models;

namespace DiveDestination.Controllers;

[ApiController]
[Route("api/authorization")]
public class AuthorizationController: ControllerBase
{
    List<PersonData> users = new List<PersonData> { 
        new() { Id = "usr_1", loginEmail = "Grigorieva@gmail.com",loginNumber = "81234567890", Name = "Григорьева", Age = 37, password = "asdgdhjt8@u4y5ved"},
        new() { Id = "usr_2", loginEmail = "matvey_frontender@gmail.com", Name = "Матвей фронтендер", Age = 10, password = "asfaer67u5nktu("},
        new() { Id = "usr_3", loginEmail = "maksim_bekender@gmail.com", loginNumber = "80987654321", Name = "Максим Бекендер", Age = 24, password = "b9s9573@(74tnv"}
    };
    private readonly ILogger<AuthorizationController> _logger;
    private readonly ApplicationContext _context;

    public AuthorizationController(ILogger<AuthorizationController> logger, ApplicationContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpPost("user")]
    public IActionResult AuthorizeUser([FromForm] string login, [FromForm] string password)
    {
        _logger.LogInformation($"Вызван эндпоинт для авторизации\n----------\nпереданные данные: \nлогин: {login} \nпароль:{password}");
        try
        {
            Persons? person = _context.Persons.FirstOrDefault(p => p.email == login && p.password == password);
            
            PersonData result = new PersonData();
        
            if (CheckData.checkCurrentEmailAddress(login))
                result = GetDataPerson.getIdLoginEmailAddress(users, login, password);
            else
            {
                var problem = new ProblemDetails {
                    Status = 403,
                    Title = "Forbidden",
                    Detail = "Логин или пароль введен не корректно!"
                };

                return Problem(problem.Detail, null, problem.Status, problem.Title);
            }
        
            var claims = new List<Claim> {new Claim(ClaimTypes.Name, result.Name), new Claim(ClaimTypes.Email, result.loginEmail) };
            // создаем JWT-токен
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
                email = result.loginEmail
            };

            return Ok(res);
        }
        catch (InvalidDataException ex)
        {
            var problemDetails = new ProblemDetails {
                Status = 403,
                Title = "Forbidden",
                Detail = ex.Message
            };

            return Problem(problemDetails.Detail, null, problemDetails.Status, problemDetails.Title);
        }
    }
}