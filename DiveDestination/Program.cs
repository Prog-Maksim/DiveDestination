using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DiveDestination;
using DiveDestination.Scripts;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов CORS
builder.Services.AddCors(); 

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = AuthOptions.ISSUER,
        ValidateAudience = true,
        ValidAudience = AuthOptions.AUDIENCE,
        ValidateLifetime = true,
        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
        ValidateIssuerSigningKey = true
    };
});

var app = builder.Build();
app.UseCors(builder => builder.AllowAnyOrigin());

List<PersonData> users = new List<PersonData> 
{ 
    new() { Id = "usr_1", loginEmail = "Grigorieva@gmail.com",loginNumber = "81234567890", Name = "Григорьева", Age = 37, password = "asdgdhjt8@u4y5ved"},
    new() { Id = "usr_2", loginEmail = "matvey_frontender@gmail.com", Name = "Матвей фронтендер", Age = 10, password = "asfaer67u5nktu("},
    new() { Id = "usr_3", loginEmail = "maksim_bekender@gmail.com", loginNumber = "80987654321", Name = "Максим Бекендер", Age = 24, password = "b9s9573@(74tnv"}
};

app.MapGet("/", () => "Welcome in Dive Destination");


app.MapPost("/api/authorization/user", (string login, string password, HttpRequest request, HttpResponse response, ILogger<Program> logger) =>
{
    logger.LogInformation($"вывзан endPoint для авторизации\n----------\nпереданные данные: \nлогин: {login} \nпароль:{password}");
    try
    {
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

            return Results.Problem(problem);
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

        return Results.Json(res);
    }
    catch (InvalidDataException ex)
    {
        var problemDetails = new ProblemDetails {
            Status = 403,
            Title = "Forbidden",
            Detail = ex.Message
        };

        return Results.Problem(problemDetails);
    }
});

app.MapPost("/api/registration/user", (string name, string login, string password, HttpRequest request, HttpResponse response, ILogger<Program> logger) =>
{
    logger.LogInformation($"вывзан endPoint для регистрации \n----------\nпереданные данные: \nимя: {name} \nлогин: {login} \nпароль: {password}");
    
    if (!request.Cookies.ContainsKey("personId"))
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

                return Results.Problem(problem);
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

            return Results.Json(res);
        }

        return Results.BadRequest(new { message = "логин или пароль не соответствует требованиям!" });
    }

    return Results.Ok(new { message = "вы уже авторизованы!" });
});

app.Run();