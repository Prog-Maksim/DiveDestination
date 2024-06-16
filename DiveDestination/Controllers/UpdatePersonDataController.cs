using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server_Test.Models;

namespace DiveDestination.Controllers;

[ApiController]
[Route("api/update-data")]
public class UpdatePersonDataController(ILogger<AuthorizationController> logger, ApplicationContext context): ControllerBase
{
    private readonly PasswordHasher<Persons> _passwordHasher = new();

    [Authorize]
    [HttpPut("password")]
    public async Task<IActionResult> UpdatePasswordUser([FromForm] string oldPassword, [FromForm] string newPassword)
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
        
        var result = _passwordHasher.VerifyHashedPassword(person, person.password, oldPassword);

        if (result == PasswordVerificationResult.Success)
        {
            person.password = _passwordHasher.HashPassword(person, newPassword);
            
            await context.SaveChangesAsync();
            
            var res = new
            {
                message = "пароль успешно обновлен"
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
    [HttpPut("photo-account")]
    public async Task<IActionResult> PhotoUser(IFormFile file)
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
        
        var allowedExtensions = new[] { ".png", ".svg", ".jpg", ".jpeg" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            var errorResponse = new { message = "Недопустимый тип файла. Разрешенные типы: png, svg, jpg, jpeg." };
            return BadRequest(errorResponse);
        }
        
        var uploadPath = $"{Directory.GetCurrentDirectory()}/ProfileImage";
        
        string fullPath = $"{uploadPath}/{Path.GetRandomFileName()}{extension}";
        
        using (var fileStream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        person.image_path = fullPath;
        await context.SaveChangesAsync();
        
        var res = new { message = "фотография профиля успешно обновлена" };
        return Ok(res);
    }

    [Authorize]
    [HttpDelete("delete-account")]
    public async Task<IActionResult> DeleteUser([FromForm] string password)
    {
        var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
        var email = emailClaim?.Value;

        var person = await context.Persons.Include(p => p.PassData).Include(p => p.UserLevel).SingleOrDefaultAsync(p => p.email == email);
        
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
            context.Persons.Remove(person);
            context.PassData.Remove(person.PassData);
            context.UserLevel.Remove(person.UserLevel);
            await context.SaveChangesAsync();
            
            var res = new
            {
                message = "Пользователь был успешно удален!"
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
}