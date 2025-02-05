using Domain.Entities.CardData;
using Domain.Entities.JMDict;

namespace Application.Utilities;

public static class EntryToCard
{
    /// <summary>
    /// Converts Entry to Card based on Entry's default parameters
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    public static Card Convert(Entry entry)
    {
        if(entry == null) throw new Exception("entry is null");
        var card = new Card();
        card.ent_seq = entry.ent_seq;
        card.Senses = entry.Senses;

        // if any keb
        if (entry.KanjiElements.Count > 0)
        {
            // try find keb with pri
            var kanjiWithPris = entry.KanjiElements.FindAll(ke => ke.ke_pri is not null).OrderBy(ke => ke.ke_pri);
            var kanjiWithPri = kanjiWithPris.FirstOrDefault();
            if (kanjiWithPri is not null)
            {
                // pri found so choose kanji with highest pri
                card.KanjiElement = kanjiWithPri;
            }
            else
            {
                // No pri found, then choose first kanji
                card.KanjiElement = entry.KanjiElements.First();
            }
            card.KanjiId = card.KanjiElement.Id;
            
            // continue to build card.ReadingElement but with Kanji element
            if(entry.ReadingElements.Count == 0) throw new ApplicationException("No reading elements found");
            
            // check if entry contains chosen keb in re_restr
            var readingWithRestr = entry.ReadingElements.Find(re => re.re_restr.Contains(card.KanjiElement.keb));
            if (readingWithRestr is not null)
            {
                card.ReadingElement = readingWithRestr;
            }
            else
            {
                // Choose first reading
                card.ReadingElement = entry.ReadingElements.First();
            }
        }
        else
        {
            // no kanji, then only reading
            if(entry.ReadingElements.Count == 0) throw new ApplicationException("No reading elements found");
            var readingWithPris = entry.ReadingElements.FindAll(re => re.re_pri is not null).OrderBy(re => re.re_pri);
            var readingWithPri = readingWithPris.FirstOrDefault();
            if (readingWithPri is not null)
            {
                // pri found so choose reading with highest pri
                card.ReadingElement = readingWithPri;
            }
            else
            {
                // no pri found, then choose first reading
                card.ReadingElement = entry.ReadingElements.First();
            }
        }

        card.ReadingId = card.ReadingElement.Id;

        return card;
    }
}