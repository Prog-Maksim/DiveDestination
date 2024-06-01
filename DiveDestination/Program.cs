using DiveDestination;
using DiveDestination.Scripts;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

List<PersonData> users = new List<PersonData> 
{ 
    new() { Id = "usr_1", loginNumber = "81234567890", Name = "Григорьева", Age = 37, password = "asdgdhjt8@u4y5ved"},
    new() { Id = "usr_2", loginEmail = "matvey_frontender@gmail.com", Name = "Матвей фронтендер", Age = 10, password = "asfaer67u5nktu("},
    new() { Id = "usr_3", loginEmail = "maksim_bekender@gmail.com", loginNumber = "80987654321", Name = "Максим Бекендер", Age = 24, password = "b9s9573@(74tnv"}
};

app.MapGet("/", () => "Welcome in Dive Destination");


app.MapPost("/api/authorization/user", (string login, string password, HttpRequest request, HttpResponse response) =>
{
    try
    {
        List<string> result = new List<string>();

        if (CheckData.checkCurrentNumberPhone(login))
            result = GetDataPerson.getIdLoginPhoneNumber(users, login);
        else if (CheckData.checkCurrentEmailAddress(login))
            result = GetDataPerson.getIdLoginEmailAddress(users, login);
        else
        {
            var problem = new ProblemDetails {
                Status = 403,
                Title = "Forbidden",
                Detail = "Логин или пароль введен не корректно!"
            };

            return Results.Problem(problem);
        }

        if (result[1] == password)
        {
            response.Cookies.Append("personId", result[0]);
            return Results.Text($"Вы успешно авторизовались, {result[2]}");
        }

        var problemDetails = new ProblemDetails {
            Status = 403,
            Title = "Forbidden",
            Detail = "логин или пароль не верен!"
        };

        return Results.Problem(problemDetails);
    }
    catch (InvalidDataException ex)
    {
        return Results.NotFound(ex.Message);
    }
});

app.MapPost("/api/registration/user", (string name, string login, string password, HttpRequest request, HttpResponse response) =>
{
    if (!request.Cookies.ContainsKey("personId"))
    {
        Random rnd = new Random();
        PersonData data = new PersonData();
        int num = rnd.Next(11111, 99999);
        data.Id = $"usr_{num}";
        data.Name = name;
        data.password = password;
        
        if (CheckData.checkCurrentNumberPhone(login) && CheckData.checkCurrentPassword(password))
        {
            if (CheckData.checkDublicatePersonDataNumberPhone(users, login))
            {
                var problem = new ProblemDetails {
                    Status = 403,
                    Title = "Forbidden",
                    Detail = "данный номер телефона уже используется!"
                };
            
                return Results.Problem(problem);
            }
                
            data.loginNumber = login;
            users.Add(data);
            
            response.Cookies.Append("personId", $"usr_{num}");
            return Results.Text($"вы успешно создали аккаунт, {name}!");
        }
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
            
            response.Cookies.Append("personId", $"usr_{num}");
            return Results.Text($"вы успешно создали аккаунт, {name}!");
        }

        var problemDetails = new ProblemDetails {
            Status = 403,
            Title = "Forbidden",
            Detail = "логин или пароль не соответствует требованиям!"
        };

        return Results.Problem(problemDetails);
    }

    return Results.Text("вы уже авторизованы!");
});

app.Run();