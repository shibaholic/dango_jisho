using Application.Mappings.EntityDtos.Tracking;
using Application.Response;
using AutoMapper;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.UseCaseQueries;

// "using" Alias Directive, change the generic type to suit the Handler return type.
using Response = Response<TrackedEntryDto>;

public record TrackedEntryIdGetRequest : IRequest<Response>
{
    public string ent_seq { get; init; }
    public Guid UserId { get; init; }
}

public class TrackedEntryIdGet : IRequestHandler<TrackedEntryIdGetRequest, Response>
{
    private readonly ITrackingRepository _trackingRepo;
    private readonly IMapper _mapper;

    public TrackedEntryIdGet(ITrackingRepository trackingRepo, IMapper mapper)
    {
        _trackingRepo = trackingRepo;
        _mapper = mapper;
    }
    
    public async Task<Response> Handle(TrackedEntryIdGetRequest request, CancellationToken cancellationToken)
    {
        var result = await _trackingRepo.ReadTrackedEntryByIdsAsync(request.ent_seq, request.UserId);
        if (result == null) return Response.NotFound("Entry not found");
        
        var dto = _mapper.Map<TrackedEntryDto>(result);
        return Response.Ok("Tracked Entry found", dto);
    }
}