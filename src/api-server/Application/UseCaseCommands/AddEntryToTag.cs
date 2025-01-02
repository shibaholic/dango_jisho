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
    private readonly IMapper _mapper;

    public AddEntryToTag(ITagRepository tagRepo, IEntryRepository entryRepo, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _entryRepo = entryRepo;
        _tagRepo = tagRepo;
        _mapper = mapper;
    }

    public async Task<Response> Handle(AddEntryToTagRequest request, CancellationToken cancellationToken)
    {
        // first, query entry and tag to see if they exist
        var entry = await _entryRepo.GetBy_ent_seq(request.ent_seq);
        if (entry == null) throw new ProblemException("Entry not found", $"Entry with ent_seq: {request.ent_seq} does not exist.");
        
        var tag = await _tagRepo.ReadByIdUserIdAsync(request.TagId, request.UserId);
        if(tag == null) throw new ProblemException("Tag not found", $"Tag with id: {request.TagId} does not exist.");
        
        var entryIsTagged = new EntryIsTagged() { ent_seq = entry.ent_seq, TagId = tag.Id, UserOrder = tag.TotalEntries };
        await _tagRepo.CreateEntryIsTaggedAsync(entryIsTagged);

        await _unitOfWork.Commit(cancellationToken);
        
        Console.WriteLine("[AddEntryToTag] Added entry to tag");
        
        return Response.NoContent("Entry added to tag");
    }
}