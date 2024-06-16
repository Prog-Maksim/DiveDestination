namespace Server_Test.Models;

public class PassData
{
    public int id { get; set; }
    public int pass_num { get; set; }
    public int pass_seria { get; set; }
    public DateTime pass_date_start { get; set; }
    public string pass_issued { get; set; }
    
    public Persons Person { get; set; }
}