using Application.Mappings.EntityDtos.Tracking;
using Application.Response;
using AutoMapper;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.UseCaseQueries;

using Response = Response<List<TagDto>>;

public record TagsByUserIdRequest : IRequest<Response>
{
    public Guid UserId { get; init; }
}

public class TagsByUserId : IRequestHandler<TagsByUserIdRequest, Response>
{
    private readonly ITagRepository _tagRepository;
    private readonly IMapper _mapper;
    
    public TagsByUserId(ITagRepository tagRepository, IMapper mapper)
    {
        _tagRepository = tagRepository;
        _mapper = mapper;
    }

    public async Task<Response> Handle(TagsByUserIdRequest request, CancellationToken cancellationToken)
    {
        var tags = await _tagRepository.ReadAllByUserIdAsync(request.UserId);
        
        var tagDtos = _mapper.Map<List<TagDto>>(tags);
        
        return Response.Ok($"{tags.Count} tags found.", tagDtos);
    }
}