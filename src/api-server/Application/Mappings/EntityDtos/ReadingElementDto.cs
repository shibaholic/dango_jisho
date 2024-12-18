namespace Application.Mappings.EntityDtos;

public class ReadingElementDto
{
    public string reb { get; set; }
    public bool re_nokanji { get; set; }
    public List<string> re_restr { get; set; }
    public List<string> re_inf { get; set; }
    public string? re_pri { get; set; }
}