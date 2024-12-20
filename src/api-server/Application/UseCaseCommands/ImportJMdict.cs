using System.Diagnostics;
using System.Xml;
using Application.Response;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.UseCaseCommands;

// "using" Alias Directive, change the generic type to suit the Handler return type.
using Response = Response<object>;

public record ImportJMdictRequest : IRequest<Response>
{
    public byte[] Content { get; init; }
}

public class ImportJMdict : IRequestHandler<ImportJMdictRequest, Response>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEntryRepository _repository;

    public ImportJMdict(IEntryRepository repository, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }
    
    public async Task<Response> Handle(ImportJMdictRequest request, CancellationToken cancellationToken)
    {
        // ToDo: Use transactions
        
        // Parse XML data in request.Content to get all Entries
        var entries = new List<Entry>();
        using var stream = new MemoryStream(request.Content);
        
        var stopwatch = Stopwatch.StartNew();
        
        using (XmlTextReader reader = new XmlTextReader(stream))
        {
            reader.DtdProcessing = DtdProcessing.Parse;
            
            // first check if the DTD DocmentType Name is "JMdict"
            bool documentTypeFound = false;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.DocumentType)
                {
                    if (reader.Name != "JMdict") throw new Exception("Only JMdict is supported.");
                    documentTypeFound = true;
                    break;
                }
            }

            if (!documentTypeFound)
            {
                throw new Exception("JMdict DTD is required.");
            }
            
            // entity Ids for KanjiElement, ReadingElement, Sense, LSource
            int k_ele_id = 0;
            int r_ele_id = 0;
            int sense_id = 0;
            int lsource_id = 0;
            
            // loop through the <JMdict> Element which contains the data
            while (reader.Read())
            {
                if (!(reader.NodeType == XmlNodeType.Element && reader.Name == "entry")) continue;
                
                var entry = new Entry();

                // Read the ent_seq
                while (reader.Read() && !(reader.NodeType == XmlNodeType.Element && reader.Name == "ent_seq")) ;
                entry.ent_seq = reader.ReadElementContentAsString();
                // Console.WriteLine($"Entry ent_seq: {entry.ent_seq}");
                
                // loop through sub-elements within <entry> that are not the <entry> EndElement
                while (reader.Read() && !(reader.NodeType == XmlNodeType.EndElement && reader.Name == "entry"))
                {
                    if (reader.NodeType != XmlNodeType.Element) continue;
                    
                    switch (reader.Name)
                    {
                        case "k_ele":
                            var k_ele = new KanjiElement();
                            k_ele.ent_seq = entry.ent_seq;
                            k_ele.Id = k_ele_id++;
                            while (reader.Read() && !(reader.NodeType == XmlNodeType.EndElement &&
                                                      reader.Name == "k_ele"))
                            {
                                if (reader.NodeType != XmlNodeType.Element) continue;
                                switch (reader.Name)
                                {
                                    case "keb":
                                        k_ele.keb = reader.ReadElementContentAsString();
                                        break;
                                    case "ke_inf":
                                        reader.Read();
                                        k_ele.ke_inf.Add(reader.Name);
                                        break;
                                    case "ke_pri":
                                        k_ele.ke_pri = reader.ReadElementContentAsString();
                                        break;
                                        
                                    default:
                                        throw new Exception($"Unknown element encountered in k_ele: {reader.NodeType} {reader.Name}");
                                }
                            }
                            
                            entry.KanjiElements.Add(k_ele);
                            break;
                        
                        case "r_ele":
                            var r_ele = new ReadingElement();
                            r_ele.ent_seq = entry.ent_seq;
                            r_ele.Id = r_ele_id++;
                            while (reader.Read() && !(reader.NodeType == XmlNodeType.EndElement &&
                                                      reader.Name == "r_ele"))
                            {
                                if (reader.NodeType != XmlNodeType.Element) continue;
                                switch (reader.Name)
                                {
                                    case "reb":
                                        r_ele.reb = reader.ReadElementContentAsString();
                                        break;
                                    case "re_nokanji":
                                        r_ele.re_nokanji = true;
                                        break;
                                    case "re_restr":
                                        r_ele.re_restr.Add(reader.ReadElementContentAsString());
                                        break;
                                    case "re_inf":
                                        reader.Read();
                                        r_ele.re_inf.Add(reader.Name);
                                        break;
                                    case "re_pri":
                                        r_ele.re_pri = reader.ReadElementContentAsString();
                                        break;
                                        
                                    default:
                                        throw new Exception($"Unknown element encountered in r_ele: {reader.NodeType} {reader.Name}");
                                }
                            }
                            
                            entry.ReadingElements.Add(r_ele);
                            break;
                        
                        case "sense":
                            var sense = new Sense();
                            sense.ent_seq = entry.ent_seq;
                            sense.Id = sense_id++;
                            while (reader.Read() && !(reader.NodeType == XmlNodeType.EndElement &&
                                                      reader.Name == "sense"))
                            {
                                if (reader.NodeType != XmlNodeType.Element) continue;
                                switch (reader.Name)
                                {
                                    case "stagk":
                                        sense.stagk.Add(reader.ReadElementContentAsString());
                                        break;
                                    case "stagr":
                                        sense.stagr.Add(reader.ReadElementContentAsString());
                                        break;
                                    case "pos":
                                        reader.Read();
                                        sense.pos.Add(reader.Name);
                                        break;
                                    case "xref":
                                        sense.xref.Add(reader.ReadElementContentAsString());
                                        break;
                                    case "ant":
                                        sense.ant.Add(reader.ReadElementContentAsString());
                                        break;
                                    case "field":
                                        reader.Read();
                                        sense.field.Add(reader.Name);
                                        break;
                                    case "misc":
                                        reader.Read();
                                        sense.misc.Add(reader.Name);
                                        break;
                                    case "s_inf":
                                        sense.s_inf.Add(reader.ReadElementContentAsString());
                                        break;
                                    case "lsource":
                                        var lsource = new LSource();
                                        lsource.SenseId = sense.Id;
                                        lsource.Id = lsource_id++;
                                        lsource.lang = reader.GetAttribute("xml:lang");
                                        lsource.ls_part = reader.GetAttribute("ls_wasei") is not null;
                                        lsource.LangValue = reader.ReadElementContentAsString();
                                        sense.lsource.Add(lsource);
                                        break;
                                    case "dial":
                                        reader.Read();
                                        sense.dial.Add(reader.Name);
                                        break;
                                    case "gloss":
                                        sense.gloss.Add(reader.ReadElementContentAsString());
                                        break;
                                        
                                    default:
                                        throw new Exception($"Unknown element encountered in sense: {reader.NodeType} {reader.Name}");
                                }
                            }
                            
                            entry.Senses.Add(sense);
                            break;
                        
                        default:
                            throw new Exception("Unknown element encountered");
                            break;
                    }
                }
                entries.Add(entry);
                
            }
        }

        stopwatch.Stop();
        Console.WriteLine($"  Parsing JMdict took {stopwatch.ElapsedMilliseconds} ms.");
        
        stopwatch.Restart();

        await _repository.BulkInsertAsync(entries);
        // await _repository.RangeCreate(entries);
        
        await _unitOfWork.Commit(cancellationToken);
        
        stopwatch.Stop();
        Console.WriteLine($"  Copying to database took {stopwatch.ElapsedMilliseconds} ms.");
        
        return Response.NoContent($"Successfully imported {entries.Count} entries.");
    }
}