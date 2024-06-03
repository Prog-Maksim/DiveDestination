namespace DiveDestination.Scripts;

public class GetDataPerson
{
    public static PersonData getIdLoginPhoneNumber(List<PersonData> data, string login)
    {
        var user = data.FirstOrDefault(d => d.loginNumber == login);
        if (user is  null)
            throw new InvalidDataException("Пользователь под данным логином не найден!");

        return user;
    }
    
    public static PersonData getIdLoginEmailAddress(List<PersonData> data, string login, string password)
    {
        var user = data.FirstOrDefault(d => d.loginEmail == login && d.password == password);
        if (user is null)
            throw new InvalidDataException("Логин или пароль не верен!");

        return user;
    }
}