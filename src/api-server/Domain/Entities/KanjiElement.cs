namespace Domain.Entities;

public class KanjiElement
{
    public int Id { get; set; }
    public string ent_seq { get; set; } // Foreign key to Entry
    public string keb { get; set; }
    public List<string> ke_inf { get; set; }
    public string? ke_pri { get; set; }

    
    public Entry Entry { get; set; }    // Nav to parent
    public KanjiElement()
    {
        ke_inf = new List<string>();
    }
}