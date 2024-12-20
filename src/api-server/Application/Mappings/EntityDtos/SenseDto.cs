using Domain.Entities;
using Domain.Entities.JMDict;

namespace Application.Mappings.EntityDtos;

public class SenseDto
{
    public List<string> stagk { get; set; }
    public List<string> stagr { get; set; }
    public List<string> pos { get; set; }
    public List<string> xref { get; set; }
    public List<string> ant { get; set; }
    public List<string> field { get; set; }
    public List<string> misc { get; set; }
    public List<string> s_inf { get; set; }
    public List<LSource> lsource { get; set; }
    public List<string> dial { get; set; }
    public List<string> gloss { get; set; }
}