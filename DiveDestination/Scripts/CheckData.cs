using System.Text.RegularExpressions;

namespace DiveDestination.Scripts;

public class CheckData
{
    public static bool checkCurrentNumberPhone(string phoneNumber)
    {
        string phonePattern = @"^[87]\d{10}$";
        bool isValidPhone = Regex.IsMatch(phoneNumber, phonePattern);
        return isValidPhone;
    }

    public static bool checkCurrentEmailAddress(string emailAddress)
    {
        string emailPattern = @"^[a-zA-Z0-9._%+-]{5,}@(?:gmail\.com|yandex\.ru)$";
        bool isValidEmail = Regex.IsMatch(emailAddress, emailPattern);
        return isValidEmail;
    }

    public static bool checkCurrentPassword(string password)
    {
        string passwordPattern = @"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[!@$%^&*()\-+<>?])[A-Za-z\d!@$%^&*()\-+<>?]{15,}$";
        bool isValidPassword = Regex.IsMatch(password, passwordPattern);
        return isValidPassword;
    }

    public static bool checkDublicatePersonDataNumberPhone(List<PersonData> data, string login)
    {
        foreach (PersonData personData in data)
        {
            if (personData.loginNumber == login)
            {
                return true;
            }
        }

        return false;
    }
    
    public static bool checkDublicatePersonDataEmailAddress(List<PersonData> data, string login)
    {
        foreach (PersonData personData in data)
        {
            if (personData.loginEmail == login)
            {
                return true;
            }
        }

        return false;
    }
}