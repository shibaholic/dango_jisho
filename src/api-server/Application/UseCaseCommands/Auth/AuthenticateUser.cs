using Application.Mappings.EntityDtos;
using AutoMapper;
using Application.Response;
using Application.Services;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.UseCaseCommands.Auth;

using Response = Response<UserAuthDto>;

public record AuthenticationRequest
(
    string Username,
    string Password
) : IRequest<Response>;

public class AuthenticateUserHandler : IRequestHandler<AuthenticationRequest, Response>
{
    private readonly IUserRepository _userRepo;
    private readonly IMapper _mapper;
    private readonly IPasswordService _password;
    private readonly IUnitOfWork _unitOfWork;
    
    public AuthenticateUserHandler(
        IUserRepository userRepo, 
        IMapper mapper, 
        IPasswordService password,
        IUnitOfWork unitOfWork)
    {
        _userRepo = userRepo;
        _mapper = mapper;
        _password = password;
        _unitOfWork = unitOfWork;
    }

    public async Task<Response> Handle(AuthenticationRequest request, CancellationToken cancellationToken)
    {
        User? user;
        try
        {
            // Search user in database
            user = await _userRepo.GetUserByUsernameAsync(request.Username, cancellationToken);
            if (user is null) return Response.Unauthorized("Invalid username or password");
            
            // Validate user password
            bool isVerified = _password.VerifyHashedPassword(user.Password, request.Password);
            if (!isVerified) return Response.Unauthorized("Invalid username or password");

            // generate new refresh token
            user.RefreshToken = Guid.NewGuid();

            await _userRepo.UpdateAsync(user);
            await _unitOfWork.Commit(cancellationToken);
        }
        catch
        {
            return Response.ServerError("Internal Server Error");
        }

        var userDto = _mapper.Map<UserDto>(user);
        UserAuthDto userAuthDto = new UserAuthDto { User = userDto, RefreshToken = user.RefreshToken };
        
        return Response.Ok("User authenticated", userAuthDto);
    }
}