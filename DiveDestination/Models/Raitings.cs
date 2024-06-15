namespace Server_Test.Models;

public class Raitings
{
    public int id { get; set; }
    public Persons person_id { get; set; }
    public int post_id { get; set; }
    public int raiting { get; set; }
    public DateTime create_date { get; set; } = DateTime.Now;
    public DateTime update_date { get; set; } = DateTime.Now;
}