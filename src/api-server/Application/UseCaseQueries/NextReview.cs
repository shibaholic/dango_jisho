using Application.Mappings.EntityDtos.JMDict;
using Application.Mappings.EntityDtos.Tracking;
using MediatR;
using Application.Response;
using AutoMapper;
using Domain.RepositoryInterfaces;

namespace Application.UseCaseQueries;

using Response = Response<TE_Entry_EITDto>;

public record NextReviewRequest : IRequest<Response>
{
    public Guid UserId { get; init; }
    public Guid TagId { get; init; }
}

public class NextReview : IRequestHandler<NextReviewRequest, Response>
{
    private readonly ITrackingRepository _trackRepo;
    private readonly IMapper _mapper;

    public NextReview(IMapper mapper, ITrackingRepository trackRepo)
    {
        _mapper = mapper;
        _trackRepo = trackRepo;
    }
    
    public async Task<Response> Handle(NextReviewRequest request, CancellationToken cancellationToken)
    {
        var response = await _trackRepo.ReadNextReview(request.UserId, request.TagId);

        if (response is null) return Response.NoContent(); // no entries due
        
        var dto = _mapper.Map<TE_Entry_EITDto>(response);
        
        return Response.Ok("Due entry found", dto);
    }
}