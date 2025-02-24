using Application.Mappings.EntityDtos;
using Application.Response;
using AutoMapper;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.UseCaseCommands.Auth;

using Response = Response<UserAuthDto>;

public record RefreshTokenRequest : IRequest<Response>
{
    public Guid RefreshToken { get; init; }
}

public class RefreshTheToken : IRequestHandler<RefreshTokenRequest, Response>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RefreshTheToken(IUserRepository userRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // TODO: implement multi-device refresh code support
    // TODO: implement refresh token expiry on the backend (delete expired tokens)
    public async Task<Response> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        try
        {
            User? user = await _userRepository.GetUserByRefreshCode(request.RefreshToken, cancellationToken);
            if (user is null)
            {
                return Response.Unauthorized("Refresh token not valid");
            }

            // Generate a new refresh token
            user.GenerateRefreshToken();
            
            await _userRepository.UpdateAsync(user);
            
            // Commit changes to database
            await _unitOfWork.Commit(cancellationToken);
            
            // Mapper user to dto
            UserAuthDto userAuthDto = _mapper.Map<UserAuthDto>(user);

            return Response.Ok("Token refreshed", userAuthDto);
        }
        catch(Exception e)
        {
            Console.Error.WriteLine(e);
            return Response.ServerError("Internal server error");
        }
    }
}