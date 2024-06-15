namespace Server_Test.Models;

public class Comments
{
    public int id { get; set; }
    public Persons person_id { get; set; }
    public int post_id { get; set; }
    public string comments { get; set; }
    public DateTime create_date { get; set; } = DateTime.Now;
}