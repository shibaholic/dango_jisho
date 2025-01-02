using Application.Mappings.EntityDtos;
using Application.Mappings.EntityDtos.Tracking;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.JMDict;
using Domain.Entities.Tracking;

namespace Application.Mappings;

public class Mappings : Profile
{
    public Mappings()
    {
        // JMDict
        CreateMap<Entry, EntryDto>();
        CreateMap<KanjiElement, KanjiElementDto>();
        CreateMap<ReadingElement, ReadingElementDto>();
        CreateMap<Sense, SenseDto>();
        CreateMap<LSource, LSourceDto>();
        
        // Tracking
        CreateMap<Tag, TagDto>();
    }
}