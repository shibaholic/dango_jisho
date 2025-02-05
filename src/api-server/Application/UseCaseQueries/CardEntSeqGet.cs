using Application.Mappings.EntityDtos.CardData;
using Application.Mappings.EntityDtos.JMDict;
using Application.Response;
using AutoMapper;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.UseCaseQueries;

using Response = Response<CardDto>;

public record CardEntSeqGetRequest : IRequest<Response>
{
    public string ent_seq { get; init; }
}

public class CardEntSeqGet : IRequestHandler<CardEntSeqGetRequest, Response>
{
    private readonly ICardRepository _cardRepo;
    private readonly IMapper _mapper;

    public CardEntSeqGet(ICardRepository cardRepo, IMapper mapper)
    {
        _cardRepo = cardRepo;
        _mapper = mapper;
    }
    
    public async Task<Response> Handle(CardEntSeqGetRequest request, CancellationToken cancellationToken)
    {
        var result = await _cardRepo.ReadByEntSeq(request.ent_seq);
        if (result == null) return Response.NotFound("Entry not found");
        
        var dto = _mapper.Map<CardDto>(result);
        return Response.Ok("Entry found", dto);
    }
}