using System.Text;
using Domain.Entities.Tracking;

namespace Domain.Entities.JMDict;

public class Entry : IBaseEntity
{
    public string ent_seq { get; set; }
    public List<KanjiElement> KanjiElements { get; set; } = new List<KanjiElement>();
    public List<ReadingElement> ReadingElements { get; set; } = new List<ReadingElement>();
    public List<Sense> Senses { get; set; } = new List<Sense>();
    public List<TrackedEntry> TrackedEntries { get; set; } = new List<TrackedEntry>(); // navigation. not included in json

    public override string ToString()
    {
        StringBuilder output = new StringBuilder();
        
        output.Append($"Entry ent_seq: {ent_seq}\n");
        
        foreach (var kE in KanjiElements)
        {
            output.Append($"  Kanji keb: {kE.keb}\n");
            if (kE.ke_pri is not null) output.Append($"    ke_pri: {kE.ke_pri}\n");
            output.Append(PrintList(kE.ke_inf, "ke_inf", "    "));
        }

        foreach (var rE in ReadingElements)
        {
            output.Append($"  Reading reb: {rE.reb}\n");
            if (rE.re_nokanji) output.Append("    re_nokanji: true\n");
            output.Append(PrintList(rE.re_restr, "re_restr", "    "));
            output.Append(PrintList(rE.re_inf, "re_inf", "    "));
            if (rE.re_pri is not null) output.Append($"    re_pri: {rE.re_pri}\n");
        }

        foreach (var sense in Senses)
        {
            output.AppendLine(sense.ToString());
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