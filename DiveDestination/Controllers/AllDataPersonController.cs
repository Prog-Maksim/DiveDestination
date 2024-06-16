using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server_Test.Models;

namespace DiveDestination.Controllers;

[ApiController]
[Route("api/user")]
public class AllDataPersonController(ILogger<RegistrationController> logger, ApplicationContext context): ControllerBase
{
    private readonly PasswordHasher<Persons> _passwordHasher = new();
    
    [Authorize]
    [HttpGet("get-data")]
    public async Task<IActionResult> DataUser()
    {
        var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
        var email = emailClaim?.Value;

        var person = await context.Persons.SingleOrDefaultAsync(p => p.email == email);

        if (person == null)
        {
            var problem = new ProblemDetails {
                Status = 404,
                Title = "Not Found",
                Detail = "Данный пользователь не найден!"
            };

            return Problem(problem.Detail, null, problem.Status, problem.Title);
        }

        string years = null;
        if (person.age != null)
        {
            TimeSpan? timeDifference = DateTime.Now - person.age;
            if (timeDifference.HasValue)
            {
                double totalYears = timeDifference.Value.TotalDays / 365.24219878;
                years = ((int)Math.Floor(totalYears)).ToString();
            }
        }
        
        var res = new
        {
            message = "успешно",
            first_name = person.first_name,
            last_name = person.last_name,
            patronymic = person.patronymic,
            age = years,
            email = person.email,
            numberphone = person.numberphone,
            status = person.status
        };

        return Ok(res);
        
    }

    [Authorize]
    [HttpGet("get-password-data")]
    public async Task<IActionResult> DataPassportUser([FromForm] string password)
    {
        var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
        var email = emailClaim?.Value;

        var person = await context.Persons.Include(p => p.PassData).SingleOrDefaultAsync(p => p.email == email);

        if (person == null)
        {
            var problem = new ProblemDetails {
                Status = 404,
                Title = "Not Found",
                Detail = "Данный пользователь не найден!"
            };

            return Problem(problem.Detail, null, problem.Status, problem.Title);
        }
        
        var result = _passwordHasher.VerifyHashedPassword(person, person.password, password);

        if (result == PasswordVerificationResult.Success)
        {
            var res = new
            {
                message = "успешно",
                pass_num = person.PassData.pass_num,
                pass_seria = person.PassData.pass_seria,
                pass_issued = person.PassData.pass_issued,
                pass_date = person.PassData.pass_date_start
            };

            return Ok(res);
        }
        var problemDetails = new ProblemDetails {
            Status = 403,
            Title = "Forbidden",
            Detail = "Пароль не верен!"
        };

        return Problem(problemDetails.Detail, null, problemDetails.Status, problemDetails.Title);
    }
    
    [Authorize]
    [HttpGet("get-image-profile")]
    public async Task<IActionResult> ImageProfileUser()
    {
        var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
        var email = emailClaim?.Value;

        var person = await context.Persons.SingleOrDefaultAsync(p => p.email == email);
        
        if (person == null)
        {
            var problem = new ProblemDetails {
                Status = 404,
                Title = "Not Found",
                Detail = "Данный пользователь не найден!"
            };

            return Problem(problem.Detail, null, problem.Status, problem.Title);
        }
        
        await Response.SendFileAsync(person.image_path);
        return new EmptyResult();
    }
}