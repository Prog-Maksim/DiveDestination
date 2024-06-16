namespace Server_Test.Models;

public class Roles
{
    public int id { get; set; }
    public string role_name { get; set; }

    public UserLevel UserLevel { get; set; }
}