using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiveDestination.Controllers;

[ApiController]
[Route("api/user")]
public class AllDataPersonController(ILogger<RegistrationController> logger, ApplicationContext context): ControllerBase
{
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
        var res = new
        {
            message = "успешно",
            first_name = person.first_name,
            last_name = person.last_name,
            patronymic = person.patronymic,
            age = (person.age == 0)? null: person.age.ToString(),
            email = person.email,
            numberphone = person.numberphone,
            status = person.status
        };

        return Ok(res);
        
    }
}