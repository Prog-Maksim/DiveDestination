namespace Server_Test.Models;

public class Persons
{
    public int id { get; set; }
    public string last_name { get; set; }
    public string first_name { get; set; }
    public string patronymic { get; set; }
    public int age { get; set; }
    public int passport { get; set; }
    public string image_path { get; set; }
    public string email { get; set; }
    public string numberphone { get; set; } = "";
    public DateTime start_registration { get; set; } = DateTime.Now;
    public bool status { get; set; } = true;
    public int user_level { get; set; }
    public string location { get; set; } = "";
    public string password { get; set; }
}