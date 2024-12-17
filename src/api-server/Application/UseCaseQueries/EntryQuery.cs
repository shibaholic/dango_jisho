using Application.Automapper.EntityDtos;
using Application.Response;
using AutoMapper;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.UseCaseQueries;

// "using" Alias Directive, change the generic type to suit the Handler return type.
using Response = Response<EntryDto>;

public record EntryQueryRequest : IRequest<Response>
{
    public string ent_seq { get; init; }
}

public class EntryQuery : IRequestHandler<EntryQueryRequest, Response>
{
    private readonly IEntryRepository _entryRepo;
    private readonly IMapper _mapper;

    public EntryQuery(IEntryRepository entryRepo, IMapper mapper)
    {
        _entryRepo = entryRepo;
        _mapper = mapper;
    }
    
    public async Task<Response> Handle(EntryQueryRequest request, CancellationToken cancellationToken)
    {
        var result = await _entryRepo.GetBy_ent_seq(request.ent_seq);

        if (result == null) return Response.NotFound("Entry not found");
        
        var dto = _mapper.Map<EntryDto>(result);
        
        return Response.Ok("Entry found", dto);
    }
}