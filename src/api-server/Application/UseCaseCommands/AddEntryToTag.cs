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

// not in use
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
        // get entry with include trackedEntry where userId match
        var entryWithTracked = await _entryRepo.ReadByEntSeqIncludeTracked(request.ent_seq, request.UserId);
        if (entryWithTracked == null) throw new ProblemException("Entry not found", $"Entry with ent_seq: {request.ent_seq} does not exist.");
        
        // if trackedEntry does not exist then create trackedEntry
        var trackedEntry = entryWithTracked.TrackedEntries.FirstOrDefault();
        if (trackedEntry == null)
        {
            trackedEntry = new TrackedEntry(entryWithTracked, request.UserId);
            // Persist TrackedEntry to repo
            await _trackingRepo.CreateAsync(trackedEntry);
        }
        
        // get the Tag object
        var tag = await _tagRepo.ReadByIdUserIdAsync(request.TagId, request.UserId);
        if (tag == null) throw new ProblemException("Tag not found", $"Tag with id: {request.TagId} does not exist.");
        
        // add trackedEntry to Tag
        tag.TryAddTrackedEntry(trackedEntry);

        // Persist the new EntryIsTagged
        var entryIsTagged = tag.EntryIsTaggeds.First(eit => eit.TagId == tag.Id && eit.ent_seq == trackedEntry.ent_seq);
        await _trackingRepo.CreateEntryIsTaggedAsync(entryIsTagged);
        
        // update tag
        await _tagRepo.UpdateAsync(tag);
        
        await _unitOfWork.Commit(cancellationToken);
        
        return Response.NoContent();
    }
}