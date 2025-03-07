using System.Text;
using Domain.RepositoryInterfaces;
using MediatR;
using Application.Response;
using AutoMapper;
using Domain.Entities.Tracking;

namespace Application.UseCaseCommands;

// "using" Alias Directive, change the generic type to suit the Handler return type.
using Response = Response<object>;

public record AddEntryToTagRequest : IRequest<Response>
{
    public string ent_seq { get; init; }
    public Guid TagId { get; init; }
    public Guid UserId { get; init; }
}

public class AddEntryToTag: IRequestHandler<AddEntryToTagRequest, Response>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITagRepository _tagRepo;
    private readonly IEntryRepository _entryRepo;
    private readonly ITrackingRepository _trackingRepo;
    private readonly IMapper _mapper;

    public AddEntryToTag(ITagRepository tagRepo, IEntryRepository entryRepo, ITrackingRepository trackingRepo, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _entryRepo = entryRepo;
        _tagRepo = tagRepo;
        _trackingRepo = trackingRepo;
        _mapper = mapper;
    }

    public async Task<Response> Handle(AddEntryToTagRequest request, CancellationToken cancellationToken)
    {
        // first, query entry and tag to see if they exist
        var entry = await _entryRepo.ReadByEntSeq(request.ent_seq);
        if (entry == null) throw new ProblemException("Entry not found", $"Entry with ent_seq: {request.ent_seq} does not exist.");
        
        var tag = await _tagRepo.ReadByIdUserIdAsync(request.TagId, request.UserId);
        if(tag == null) throw new ProblemException("Tag not found", $"Tag with id: {request.TagId} does not exist.");
        
        // Use tag.TotalEntries to order this EntryIsTagged to the last index
        var entryIsTagged = new EntryIsTagged() { ent_seq = entry.ent_seq, TagId = tag.Id, UserOrder = tag.TotalEntries };
        await _trackingRepo.CreateEntryIsTaggedAsync(entryIsTagged);
        
        // Update tag.TotalEntries to += 1
        tag.TotalEntries += 1;
        
        // If this user does not already track this entry, then add TrackedEntry entity so it is tracked
        var trackedEntry = await _trackingRepo.ReadTrackedEntryByIdsAsync(entry.ent_seq, request.UserId);
        if (trackedEntry == null)
        {
            trackedEntry = new TrackedEntry
            {
                ent_seq = entry.ent_seq, UserId = request.UserId, NextReviewDays = null, NextReviewMinutes = null, 
                Score = 0, LevelStateType = LevelStateType.New
            };
            await _trackingRepo.CreateTrackedEntryAsync(trackedEntry);
            Console.WriteLine("[AddEntryToTag] Added entry to user tracking.");
        }
        
        await _unitOfWork.Commit(cancellationToken);
        
        Console.WriteLine("[AddEntryToTag] Added entry to tag.");
        
        return Response.NoContent();
    }
}