using Application.Response;
using AutoMapper;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.UseCaseQueries;

public class TagsByUserIdRequest<DtoType> : IRequest<Response<List<DtoType>>>
{
    public Guid UserId { get; init; }
    public TagsByUserIdRequest(Guid userId)
    {
        UserId = userId;
    }
}

public class TagsByUserId<DtoType> : IRequestHandler<TagsByUserIdRequest<DtoType>, Response<List<DtoType>>>
{
    private readonly ITagRepository _tagRepository;
    private readonly IMapper _mapper;
    
    public TagsByUserId(ITagRepository tagRepository, IMapper mapper)
    {
        _tagRepository = tagRepository;
        _mapper = mapper;
    }

    public async Task<Response<List<DtoType>>> Handle(TagsByUserIdRequest<DtoType> request, CancellationToken cancellationToken)
    {
        var tags = await _tagRepository.ReadAllByUserIdAsync(request.UserId);
        
        var tagDtos = _mapper.Map<List<DtoType>>(tags);
        
        return Response<List<DtoType>>.Ok($"{tags.Count} tags found.", tagDtos);
    }
}