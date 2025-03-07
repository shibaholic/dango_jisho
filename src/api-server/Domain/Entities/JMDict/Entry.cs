using System.Text;
using Domain.Entities.Tracking;

namespace Domain.Entities.JMDict;

public class Entry : IBaseEntity
{
    public string ent_seq { get; set; } // TODO: to value object
    public int? SelectedKanjiIndex { get; set; } = null;
    public int SelectedReadingIndex { get; set; }
    public int? PriorityScore { get; set; } = null;
    public List<KanjiElement> KanjiElements { get; set; } = new List<KanjiElement>();
    public List<ReadingElement> ReadingElements { get; set; } = new List<ReadingElement>();
    public List<Sense> Senses { get; set; } = new List<Sense>();
    public List<TrackedEntry> TrackedEntries { get; set; } = new List<TrackedEntry>(); // navigation. not included in json

    // TODO: maybe I should just scrap this, because the defaults in JMDict (first kanji and first reading) seems to be fine.
    public Entry SetDefaultSelectedIndexes()
    {
        if (KanjiElements.Count > 0)
        {
            // try find keb with pri
            var selectedKanji = KanjiElements.Where(ke => ke.ke_pri is not null).OrderBy(ke => ke.ke_pri).Select((ke, index) => new {ke, index}).FirstOrDefault();
            if (selectedKanji is not null)
            {
                // pri found so choose kanji with highest pri
                SelectedKanjiIndex = selectedKanji.index;
            }
            else
            {
                // No pri found, then choose first kanji
                SelectedKanjiIndex = KanjiElements.Select((ke, index) => index).First();
            }
            
            // continue to build card.ReadingElement but with Kanji element
            if(ReadingElements.Count == 0) throw new ApplicationException("No reading elements found");
            
            // check if entry contains chosen keb in re_restr
            // var readingWithRestr = entry.ReadingElements.Find(re => re.re_restr.Contains(card.KanjiElement.keb));
            int? readingWithRestr = ReadingElements.Where(re => re.re_restr.Contains(KanjiElements[(int)SelectedKanjiIndex].keb)).Select((re, index) => index).FirstOrDefault();
            
            if (readingWithRestr is not null)
            {
                SelectedReadingIndex = (int)readingWithRestr;
            }
            else
            {
                // Choose first reading
                SelectedReadingIndex = ReadingElements.Select((re, index) => index).First();
            }
        }
        else
        {
            // no kanji, then only reading
            if(ReadingElements.Count == 0) throw new ApplicationException("No reading elements found");
            int? selectedReadingIndex = ReadingElements.Where(re => re.re_pri is not null).OrderBy(re => re.re_pri).Select((ke, index) => index).FirstOrDefault();
            if (selectedReadingIndex is not null)
            {
                // pri found so choose reading with highest pri
                SelectedReadingIndex = (int)selectedReadingIndex;
            }
            else
            {
                // no pri found, then choose first reading
                SelectedReadingIndex = ReadingElements.Select((re, index) => index).First();
            }
        }

        if (SelectedReadingIndex == -1)
        {
            Console.WriteLine("SelectedReadingIndex is null");
            Console.WriteLine($"ent_seq: {ent_seq}");
            throw new Exception();
        }
        
        return this;
    }

    public Entry SetPriorityScore()
    {
        var firstKePri = KanjiElements.OrderByDescending(ke => ke.ke_pri.HasValue)
            .ThenBy(ke => ke.ke_pri)
            .Select(ke => ke.ke_pri).FirstOrDefault();

        if (firstKePri is not null)
        {
            PriorityScore = (int)firstKePri;
            return this;
        }
        
        var firstRePri = ReadingElements.OrderByDescending(re => re.re_pri.HasValue)
            .ThenBy(re => re.re_pri)
            .Select(re => re.re_pri).FirstOrDefault();
        
        if (firstRePri is not null)
        {
            PriorityScore = (int)firstRePri;
            return this;
        }
        
        // no ke_pri or re_pri, so I guess its not common
        PriorityScore = Int32.MaxValue;
        return this;
    }
    
    public override string ToString()
    {
        StringBuilder output = new StringBuilder();
        
        output.Append($"Entry ent_seq: {ent_seq}\n");
        output.AppendLine($"SelectedKanjiIndex: {SelectedKanjiIndex}");
        output.AppendLine($"SelectedReadingIndex: {SelectedReadingIndex}");
        output.AppendLine($"PriorityScore: {PriorityScore}");
        
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