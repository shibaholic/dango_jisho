using System.Diagnostics;
using Application.Mappings.EntityDtos;
using Application.Mappings.EntityDtos.CardData;
using Application.Mappings.EntityDtos.JMDict;
using Application.Response;
using AutoMapper;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.UseCaseQueries;

using Response = Response<List<CardDto>>;

public record CardQueryRequest : IRequest<Response>
{
    public string query { get; init; }
}

public class CardQuery : IRequestHandler<CardQueryRequest, Response>
{
    private readonly ICardRepository _cardRepo;
    private readonly IMapper _mapper;

    public CardQuery(ICardRepository cardRepo, IMapper mapper)
    {
        _cardRepo = cardRepo;
        _mapper = mapper;
    }
    
    public async Task<Response> Handle(CardQueryRequest request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await _cardRepo.Search(request.query);
        stopwatch.Stop();
        
        Console.WriteLine($"Card Search Query time: {stopwatch.ElapsedMilliseconds} ms.");
        
        var dto = _mapper.Map<List<CardDto>>(result);
        return Response.Ok("Query results", dto);
    }
}