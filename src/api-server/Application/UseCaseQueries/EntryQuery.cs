using Application.Response;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.UseCaseQueries;

// "using" Alias Directive, change the generic type to suit the Handler return type.
// using Response = Response<Entry>;

public record EntryQueryRequest : IRequest<Response<Entry>>
{
    public string ent_seq { get; init; }
}

public class EntryQuery : IRequestHandler<EntryQueryRequest, Response<Entry>>
{
    private readonly IEntryRepository _entryRepo;

    public EntryQuery(IEntryRepository entryRepo)
    {
        _entryRepo = entryRepo;
    }
    
    public async Task<Response<Entry>> Handle(EntryQueryRequest request, CancellationToken cancellationToken)
    {
        var result = await _entryRepo.GetBy_ent_seq(request.ent_seq);

        if (result == null) return Response<Entry>.NotFound("Entry not found");
        
        return Response<Entry>.Success("Entry found", result);
    }
}