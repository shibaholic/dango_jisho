namespace Domain.Entities;

public class Entry : IBaseEntity
{
    public Entry()
    {
    }

    public Entry(string entSeq)
    {
        ent_seq = entSeq;
    }

    public string ent_seq { get; set; }
}