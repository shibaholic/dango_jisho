using System.Text;

namespace Domain.Entities;

public class Entry : IBaseEntity
{
    public string ent_seq { get; set; }
    public List<int> KanjiElementIds { get; set; }
    public List<KanjiElement> KanjiElementNavs { get; set; }
    public List<int> ReadingElementIds { get; set; }
    public List<ReadingElement> ReadingElementNavs { get; set; }
    public List<Sense> SensesNav { get; set; }
    public List<int> Senses { get; set; }
    public Entry()
    {
        KanjiElementNavs = new List<KanjiElement>();
        ReadingElementNavs = new List<ReadingElement>();
        SensesNav = new List<Sense>();
    }

    public Entry(string entSeq)
    {
        ent_seq = entSeq;
    }

    public override string ToString()
    {
        StringBuilder output = new StringBuilder();
        
        output.Append($"Entry ent_seq: {ent_seq}\n");
        
        foreach (var kE in KanjiElementNavs)
        {
            output.Append($"  Kanji keb: {kE.keb}\n");
            if (kE.ke_pri is not null) output.Append($"    ke_pri: {kE.ke_pri}\n");
            // if (kE.ke_inf.Count != 0)
            // {
            //     output.Append("    ke_inf: ");
            //     foreach (var ke_inf in kE.ke_inf)
            //     {
            //         output.Append($"{ke_inf}, ");
            //     }
            //     output.Remove(output.Length - 2, 2);
            //     output.Append("\n");
            // }
            output.Append(PrintList(kE.ke_inf, "ke_inf", "    "));
        }

        foreach (var rE in ReadingElementNavs)
        {
            output.Append($"  Reading reb: {rE.reb}\n");
            if (rE.re_nokanji) output.Append("    re_nokanji: true\n");
            output.Append(PrintList(rE.re_restr, "re_restr", "    "));
            output.Append(PrintList(rE.re_inf, "re_inf", "    "));
            if (rE.re_pri is not null) output.Append($"    re_pri: {rE.re_pri}\n");
        }

        foreach (var sense in SensesNav)
        {
            output.Append($"  Sense\n");
            output.Append(PrintList(sense.stagk, "stagk", "    "));
            output.Append(PrintList(sense.stagr, "stagr", "    "));
            output.Append(PrintList(sense.pos, "pos", "    "));
            output.Append(PrintList(sense.xref, "xref", "    "));
            output.Append(PrintList(sense.ant, "ant", "    "));
            output.Append(PrintList(sense.field, "field", "    "));
            output.Append(PrintList(sense.misc, "misc", "    "));
            output.Append(PrintList(sense.s_inf, "s_inf", "    "));
            if (sense.lsource.Count != 0)
            {
                foreach (var lsource_element in sense.lsource)
                {
                    output.Append("    " + $"lsource: {lsource_element.langValue}\n");
                    if(lsource_element.ls_part) output.Append("      part: true\n");
                    if(lsource_element.ls_wasei) output.Append("      wasei: true\n");
                }
                output.Remove(output.Length - 2, 2);
                output.Append("\n");
            }
            output.Append(PrintList(sense.dial, "dial", "    "));
            output.Append(PrintList(sense.gloss, "gloss", "    "));
        }

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