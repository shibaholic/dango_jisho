using Application.Mappings.EntityDtos;
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
        CreateMap<Entry, EntryDto>();
        CreateMap<Entry, Entry_TEDto>()
            .ForMember(dest => dest.TrackedEntry, opt => opt.MapFrom(src => src.TrackedEntries.FirstOrDefault()))
            .ReverseMap();
        CreateMap<KanjiElement, KanjiElementDto>().ReverseMap();
        CreateMap<ReadingElement, ReadingElementDto>().ReverseMap();
        CreateMap<Sense, SenseDto>().ReverseMap();
        CreateMap<LSource, LSourceDto>().ReverseMap();
        
        // Tracking
        CreateMap<Tag, Tag_EITDto>();
        CreateMap<Tag, TagDto>();
        CreateMap<EntryEvent, ReviewEventDto>();
        CreateMap<StudySet, StudySetDto>();
        CreateMap<TrackedEntry, TE_EntryDto>();
        CreateMap<TrackedEntry, TE_EITDto>();
        CreateMap<TrackedEntry, TE_Entry_EITDto>();
        CreateMap<EntryIsTagged, EIT_TEDto>();
        CreateMap<EntryIsTagged, EIT_TagDto>();
        
        // Tracking custom
        
        // Other
        CreateMap<User, UserDto>();
        CreateMap<User, UserAuthDto>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src));
    }
}