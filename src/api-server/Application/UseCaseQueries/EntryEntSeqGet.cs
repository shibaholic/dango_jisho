using Application.Mappings.EntityDtos;
using Application.Mappings.EntityDtos.JMDict;
using Application.Response;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.JMDict;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.UseCaseQueries;

// "using" Alias Directive, change the generic type to suit the Handler return type.
using Response = Response<Entry_TEDto>;

public record EntryEntSeqGetRequest : IRequest<Response>
{
    public Guid? UserId { get; init; }
    public string ent_seq { get; init; }
}

public class EntryEntSeqGet : IRequestHandler<EntryEntSeqGetRequest, Response>
{
    private readonly IEntryRepository _entryRepo;
    private readonly IMapper _mapper;

    public EntryEntSeqGet(IEntryRepository entryRepo, IMapper mapper)
    {
        _entryRepo = entryRepo;
        _mapper = mapper;
    }
    
    public async Task<Response> Handle(EntryEntSeqGetRequest request, CancellationToken cancellationToken)
    {
        Entry? result;
        if (request.UserId is null)
        {
            result = await _entryRepo.ReadByEntSeq(request.ent_seq);
        }
        else
        {
            result = await _entryRepo.ReadByEntSeqIncludeTracked(request.ent_seq, (Guid)request.UserId);
        }
        
        if (result == null) return Response.NotFound("Entry not found");
        
        var dto = _mapper.Map<Entry_TEDto>(result);
        return Response.Ok("Entry found", dto);
    }
}