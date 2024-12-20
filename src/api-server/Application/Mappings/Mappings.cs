using Application.Mappings.EntityDtos;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.JMDict;

namespace Application.Mappings;

public class Mappings : Profile
{
    public Mappings()
    {
        CreateMap<Entry, EntryDto>();
        CreateMap<KanjiElement, KanjiElementDto>();
        CreateMap<ReadingElement, ReadingElementDto>();
        CreateMap<Sense, SenseDto>();
    }
}