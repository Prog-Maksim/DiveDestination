namespace DiveDestination.Scripts;

public class GetDataPerson
{
    public static List<string> getIdLoginPhoneNumber(List<PersonData> data, string login)
    {
        foreach (var personData in data)
        {
            if (personData.loginNumber == login)
            {
                return new List<string> {personData.Id, personData.password, personData.Name};
            }
        }

        throw new InvalidDataException("Пользователь под данным логином не найден!");
    }
    
    public static List<string> getIdLoginEmailAddress(List<PersonData> data, string login)
    {
        foreach (var personData in data)
        {
            if (personData.loginEmail == login)
            {
                return new List<string> {personData.Id, personData.password, personData.Name};
            }
        }

        throw new InvalidDataException("Пользователь под данным логином не найден!");
    }
}