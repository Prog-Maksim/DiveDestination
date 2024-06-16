namespace Server_Test.Models;

public class UserLevel
{
    public int id { get; set; }
    public int level { get; set; } = 1;
    public DateTime date_start { get; set; } = DateTime.Now;
    public DateTime update_date { get; set; } = DateTime.Now;
    
    public Persons Person { get; set; }
    public Roles Roles { get; set; }
}