using System.Text;

namespace Domain.Entities;

public class Sense
{
    public int Id { get; set; }
    public string ent_seq { get; set; } // Foreign key
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

    public Entry Entry  { get; set; }   // Nav

    public Sense()
    {
        stagk = new List<string>();
        stagr = new List<string>();
        pos = new List<string>();
        xref = new List<string>();
        ant = new List<string>();
        field = new List<string>();
        misc = new List<string>();
        s_inf = new List<string>();
        lsource = new List<LSource>();
        dial = new List<string>();
        gloss = new List<string>();
    }
}

public class LSource
{
    public int Id { get; set; }
    public string LangValue { get; set; }
    public string? lang { get; set; }
    public bool ls_part { get; set; }
    public bool ls_wasei { get; set; }

    public LSource()
    {
        lang = null;
        ls_part = false;
        ls_wasei = false;
    }

    public override string ToString()
    {
        var output = new StringBuilder();
        output.Append($"lsource: {LangValue}\n");
        if(lang is not null) output.Append($"      lang: {lang}\n");
        if(ls_part) output.Append("      part: true\n");
        if(ls_wasei) output.Append("      wasei: true\n");
        return output.ToString();
    }
}