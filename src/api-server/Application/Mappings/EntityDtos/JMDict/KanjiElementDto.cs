using Domain.Enums;

namespace Application.Mappings.EntityDtos.JMDict;

public class KanjiElementDto
{
    public string ent_seq { get; set; }
    public string keb { get; set; }
    public List<string> ke_inf { get; set; }
    public string? ke_pri { get; set; }
}