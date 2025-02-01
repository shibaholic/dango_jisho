using Domain.Enums;

namespace Domain.Entities.JMDict;

public class KanjiElement
{
    public int Id { get; set; }
    public string ent_seq { get; set; } // Foreign key to Entry
    public string keb { get; set; }
    public List<string> ke_inf { get; set; } = new List<string>();
    public Priority? ke_pri { get; set; } = null;
    
    public Entry Entry { get; set; }    // Nav to parent
}