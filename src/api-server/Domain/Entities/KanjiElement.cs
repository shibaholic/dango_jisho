namespace Domain.Entities;

public class KanjiElement
{
    public int Id { get; set; }
    public string keb { get; set; }
    public List<string> ke_inf { get; set; }
    public string? ke_pri { get; set; }

    public KanjiElement()
    {
        ke_inf = new List<string>();
    }
}