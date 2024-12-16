using Application.Automapper.EntityDtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public class Mappings : Profile
{
    public Mappings()
    {
        CreateMap<Entry, EntryDto>();
    }
}