using Application.Response;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.UseCaseCommands.Auth;

using Response = Response<object>;

public record LogoutUserRequest(
    Guid UserId,
    Guid RefreshToken
) : IRequest<Response>;

public class LogoutUser: IRequestHandler<LogoutUserRequest, Response>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LogoutUser(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Response> Handle(LogoutUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _userRepository.UpdateRefreshCodeToNull(request.UserId, request.RefreshToken, cancellationToken);
            
            if (result == false) return Response.Unauthorized("Refresh token not valid");

            await _unitOfWork.Commit(cancellationToken);
            
            return Response.NoContent();
        }
        catch
        {
            Console.Error.WriteLine($"LogoutUser {request.UserId} failed.");
            return Response.ServerError("Internal Server Error");
        }
    }
}