using Domain.Entities.Tracking;
using Application.Response;
using Application.Services;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.UseCaseCommands;

// "using" Alias Directive, change the generic type to suit the Handler return type.
using Response = Response<object>;

public record AddEntryEventRequest : IRequest<Response>
{
    public string ent_seq { get; init; }
    public Guid UserId { get; init; }
    public string Value { get; init; }
    public EventType EventType { get; init; }
}

public class AddEntryEvent : IRequestHandler<AddEntryEventRequest, Response>
{
    private readonly ITrackingRepository _trackingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITimeService _time;

    public AddEntryEvent(ITrackingRepository trackingRepository, IUnitOfWork unitOfWork, ITimeService time)
    {
        _trackingRepository = trackingRepository;
        _unitOfWork = unitOfWork;
        _time = time;
    }

    public async Task<Response> Handle(AddEntryEventRequest eventRequest, CancellationToken cancellationToken)
    {
        // Convert AddEntryEventRequest to EntryEvent
        EntryEvent entryEvent;
        if (eventRequest.EventType == EventType.Change)
        {
            var successfulParse = Enum.TryParse(eventRequest.Value, out ChangeValue changeValue);
            if (!successfulParse)
                throw new ProblemException("Invalid value for Value for EventType.Change", "Value for EventType.Change must be either 'New', 'Learning', 'Reviewing', or 'Known'");
            entryEvent = new EntryEvent() { ent_seq = eventRequest.ent_seq, UserId = eventRequest.UserId, EventType = EventType.Change, ChangeValue = changeValue}; 
        } else if (eventRequest.EventType == EventType.Review)
        {
            var successfulParse = Enum.TryParse(eventRequest.Value, out ReviewValue changeValue);
            if (!successfulParse)
                throw new ProblemException("Invalid value for Value for EventType.Change", "Value for EventType.Change must be either 'New', 'Learning', 'Reviewing', or 'Known'");
            entryEvent = new EntryEvent() { ent_seq = eventRequest.ent_seq, UserId = eventRequest.UserId, EventType = EventType.Review, ReviewValue = changeValue}; 
        }
        else
        {
            throw new ProblemException("Invalid argument for EventType", "EventType has to be either Change or Review");
        }
        
        var trackedEntry = await _trackingRepository.ReadTrackedEntryByIdsAsync(eventRequest.ent_seq, eventRequest.UserId);
        if (trackedEntry == null) throw new ProblemException("Entry not found.", "");
        
        trackedEntry.SetLevelState(LevelStateFactory.Create(trackedEntry.LevelStateType));
        
        trackedEntry.UpdateBasedOnEntryEvent(entryEvent, _time.Now);

        await _trackingRepository.CreateReviewEventAsync(entryEvent);
        
        await _trackingRepository.UpdateAsync(trackedEntry);
        
        await _unitOfWork.Commit(cancellationToken);
        
        return Response.NoContent();
    }
}