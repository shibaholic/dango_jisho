using System.Diagnostics;
using Application.Mappings.EntityDtos;
using Application.Mappings.EntityDtos.JMDict;
using Application.Response;
using AutoMapper;
using Domain.Entities.JMDict;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.UseCaseQueries;

using Response = Response<List<Entry_TEDto>>;

public record EntryQueryRequest : IRequest<Response>
{
    public string query { get; init; }
    public Guid? UserId { get; init; }
}

public class EntryQuery : IRequestHandler<EntryQueryRequest, Response>
{
    private readonly IEntryRepository _entryRepo;
    private readonly IMapper _mapper;

    public EntryQuery(IEntryRepository entryRepo, IMapper mapper)
    {
        _entryRepo = entryRepo;
        _mapper = mapper;
    }
    
    /// <summary>
    /// Forwards the query request to the repository
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<Response> Handle(EntryQueryRequest request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await _entryRepo.Search(request.query, request.UserId);
        stopwatch.Stop();
        
        Console.WriteLine($"Entry Search Query time: {stopwatch.ElapsedMilliseconds} ms.");
        
        var dto = _mapper.Map<List<Entry_TEDto>>(result);
        return Response.Ok("Query results", dto);
    }
}