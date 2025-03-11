using System.Text;
using Domain.RepositoryInterfaces;
using MediatR;
using Application.Response;
using AutoMapper;
using Domain.Entities.Tracking;

namespace Application.UseCaseCommands;

// "using" Alias Directive, change the generic type to suit the Handler return type.
using Response = Response<object>;

public class TagBatchEntriesRequest : IRequest<Response>
{
    public Dictionary<Guid, bool> TagValues { get; set; } = null!;
    public string ent_seq { get; set; } = null!;
    public Guid UserId { get; set; }
}

public class BatchTagEntry: IRequestHandler<TagBatchEntriesRequest, Response>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITagRepository _tagRepo;
    private readonly IEntryRepository _entryRepo;
    private readonly ITrackingRepository _trackingRepo;
    private readonly IMapper _mapper;

    public BatchTagEntry(ITagRepository tagRepo, IEntryRepository entryRepo, ITrackingRepository trackingRepo, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _entryRepo = entryRepo;
        _tagRepo = tagRepo;
        _trackingRepo = trackingRepo;
        _mapper = mapper;
    }

    public async Task<Response> Handle(TagBatchEntriesRequest request, CancellationToken cancellationToken)
    {
        Console.WriteLine("\n");
        
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
        
        // get the Tags
        var tags = await _tagRepo.ReadMultipleTagIdAsync(request.TagValues.Keys, request.UserId);
        if (tags.Count == 0) return Response.BadRequest("No tags found");

        // create a new Dictionary<Guid, bool> with only keys from the ones found in the repo, combined with values from request.TagValues 
        var validTagValues = new Dictionary<Guid, bool>();
        foreach (var tag in tags)
        {
            validTagValues[tag.Id] = request.TagValues[tag.Id];
        }
        
        // add trackedEntry to each Tag
        foreach (var tagValue in validTagValues)
        {
            var tag = tags.First(t => t.Id == tagValue.Key);
            Console.WriteLine($"\ntag: {tag.Name}");
            if (tagValue.Value == true)
            {
                // use TryAddTrackedEntry which creates the junction entity EntryIsTagged
                var tryResult = tag.TryAddTrackedEntry(trackedEntry);
                Console.WriteLine($"add tryResult: {tryResult}");
                if (tryResult)
                {
                    // Persist the creation of EntryIsTagged
                    _tagRepo.DetectChangesAsync();
                    await _tagRepo.UpdateAsync(tag);
                }
            }
            else
            {
                // remove junction entity EntryIsTagged
                var tryResult = tag.TryRemoveTrackedEntry(trackedEntry);
                Console.WriteLine($"remove tryResult: {tryResult}");

                if (tryResult)
                {
                    // Persist the removal of EntryIsTagged
                    _tagRepo.DetectChangesAsync();
                    await _tagRepo.UpdateAsync(tag);
                }
            }
        }
        
        await _unitOfWork.Commit(cancellationToken);
        
        return Response.NoContent();
    }
}