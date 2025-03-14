using System.Text;

namespace Domain.Entities.JMDict;

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

    public override string ToString()
    {
        StringBuilder output = new StringBuilder();
        
        output.Append($"  Sense\n");
        output.Append(PrintList(stagk, "stagk", "    "));
        output.Append(PrintList(stagr, "stagr", "    "));
        output.Append(PrintList(pos, "pos", "    "));
        output.Append(PrintList(xref, "xref", "    "));
        output.Append(PrintList(ant, "ant", "    "));
        output.Append(PrintList(field, "field", "    "));
        output.Append(PrintList(misc, "misc", "    "));
        output.Append(PrintList(s_inf, "s_inf", "    "));
        if (lsource.Count != 0)
        {
            foreach (var lsource_element in lsource)
            {
                output.Append("    " + $"lsource: {lsource_element.LangValue}\n");
                if(lsource_element.ls_part) output.Append("      part: true\n");
                if(lsource_element.ls_wasei) output.Append("      wasei: true\n");
            }
            output.Remove(output.Length - 2, 2);
            output.Append("\n");
        }
        output.Append(PrintList(dial, "dial", "    "));
        output.Append(PrintList(gloss, "gloss", "    "));

        return output.ToString();
    }
    
    private string PrintList(List<string> list, string elementName, string indent = "")
    {
        StringBuilder output = new StringBuilder();
        if (list.Count != 0)
        {
            output.Append(indent + $"{elementName}: ");
            foreach (var entity in list)
            {
                output.Append($"{entity}, ");
            }
            output.Remove(output.Length - 2, 2);
            output.Append("\n");
        }

        return output.ToString();
    }
}

public class LSource
{
    public int Id { get; set; }
    public int SenseId { get; set; } // foreign key
    public string LangValue { get; set; }
    public string? lang { get; set; }
    public bool ls_part { get; set; }
    public bool ls_wasei { get; set; }
    public Sense Sense { get; set; } // parent nav

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