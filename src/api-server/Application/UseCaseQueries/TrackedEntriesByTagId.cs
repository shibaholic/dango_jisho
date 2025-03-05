using Application.Mappings.EntityDtos.Tracking;
using Application.Response;
using AutoMapper;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.UseCaseQueries;

using Response = Response<List<TrackedEntryDto>>;

public record TrackedEntriesByTagIdRequest : IRequest<Response>
{
    public Guid TagId { get; init; }
    public Guid UserId { get; init; }
    public int PageIndex { get; init; }
    public int PageSize { get; init; }
}

public class TrackedEntriesByTagId : IRequestHandler<TrackedEntriesByTagIdRequest, Response>
{
    private readonly ITrackingRepository _trackingRepo;
    private readonly IMapper _mapper;

    public TrackedEntriesByTagId(ITrackingRepository trackingRepo, IMapper mapper)
    {
        _trackingRepo = trackingRepo;
        _mapper = mapper;
    }

    public async Task<Response> Handle(TrackedEntriesByTagIdRequest request, CancellationToken cancellationToken)
    {
        var pagedResult = await _trackingRepo.ReadTrackedEntryByTagIdAsync(request.TagId, request.UserId, request.PageIndex, request.PageSize);

        var trackedEntriesDto = _mapper.Map<List<TrackedEntryDto>>(pagedResult.Data);

        if (trackedEntriesDto.Count != 0)
            return Response.OkPaginated($"Found {trackedEntriesDto.Count} entries.", trackedEntriesDto, request.PageIndex, request.PageSize, trackedEntriesDto.Count, pagedResult.TotalElements);
        else
            return Response.OkPaginated($"Found 0 entries.", [], request.PageIndex, request.PageSize, 0, pagedResult.TotalElements);
    }
}