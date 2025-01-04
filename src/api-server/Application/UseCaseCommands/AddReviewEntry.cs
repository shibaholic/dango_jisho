using Domain.Entities.Tracking;
using Application.Response;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.UseCaseCommands;

// "using" Alias Directive, change the generic type to suit the Handler return type.
using Response = Response<object>;

public record AddReviewEntryRequest : IRequest<Response>
{
    public string ent_seq { get; init; }
    public Guid UserId { get; init; }
    public ReviewValue Value { get; init; }
}

public class AddReviewEntry : IRequestHandler<AddReviewEntryRequest, Response>
{
    private readonly ITrackingRepository _trackingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddReviewEntry(ITrackingRepository trackingRepository, IUnitOfWork unitOfWork)
    {
        _trackingRepository = trackingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Response> Handle(AddReviewEntryRequest request, CancellationToken cancellationToken)
    {
        var reviewEvent = new ReviewEvent()
            { ent_seq = request.ent_seq, UserId = request.UserId, Value = request.Value };
        
        await _trackingRepository.CreateReviewEventAsync(reviewEvent);
        
        await _unitOfWork.Commit();
        
        return Response.NoContent();
    }
}