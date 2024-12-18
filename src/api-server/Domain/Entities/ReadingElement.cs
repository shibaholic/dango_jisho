namespace Domain.Entities;

public class ReadingElement
{
    public int Id { get; set; }
    public string ent_seq { get; set; } // Foreign key
    public string reb { get; set; }
    public bool re_nokanji { get; set; }
    public List<string> re_restr { get; set; }
    public List<string> re_inf { get; set; }
    public string? re_pri { get; set; }
    
    public Entry Entry  { get; set; }   // Nav

    public ReadingElement()
    {
        re_nokanji = false;
        re_restr = new List<string>();
        re_inf = new List<string>();
    }
}