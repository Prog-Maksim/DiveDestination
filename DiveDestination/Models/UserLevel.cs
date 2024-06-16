namespace Server_Test.Models;

public class UserLevel
{
    public int id { get; set; }
    public string level { get; set; }
    public DateTime date_start { get; set; } = DateTime.Now;
    public DateTime update_date { get; set; } = DateTime.Now;
    
    public Persons Person { get; set; }
}