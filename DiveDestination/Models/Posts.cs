namespace Server_Test.Models;

public class Posts
{
    public int id { get; set; }
    public DateTime create_date { get; set; } = DateTime.Now;
    public DateTime update_date { get; set; } = DateTime.Now;
    public int person_id { get; set; }
    public string title { get; set; }
    public string country { get; set; }
    public double price { get; set; }
    public double sale_price { get; set; }
    public int raitings { get; set; }
    public int comments { get; set; }
}