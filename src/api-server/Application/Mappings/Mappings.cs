using Application.Mappings.EntityDtos;
using Application.Mappings.EntityDtos.CardData;
using Application.Mappings.EntityDtos.JMDict;
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
        CreateMap<Entry, EntryDto>().ReverseMap();
        CreateMap<KanjiElement, KanjiElementDto>().ReverseMap();
        CreateMap<ReadingElement, ReadingElementDto>().ReverseMap();
        CreateMap<Sense, SenseDto>().ReverseMap();
        CreateMap<LSource, LSourceDto>().ReverseMap();
        
        // Tracking
        CreateMap<Tag, TagDto>();
        CreateMap<EntryEvent, ReviewEventDto>();
        CreateMap<StudySet, StudySetDto>();
        CreateMap<TrackedEntry, TrackedEntryDto>();
        
        // Other
        CreateMap<User, UserDto>();
    }
}