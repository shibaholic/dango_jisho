using Application.Mappings.EntityDtos;
using Application.Response;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.UseCaseCommands.Auth;

using Response = Response<UserDto>;

public record CreateUserRequest : IRequest<Response>
{
    public string Username { get; init; }
    public string Password { get; init; }
    public Guid? UserId { get; init; }
}

public class CreateUser : IRequestHandler<CreateUserRequest, Response>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _password;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateUser(
        IUserRepository userRepository, 
        IPasswordService password, 
        IUnitOfWork unitOfWork, 
        IMapper mapper)
    {
        _userRepository = userRepository;
        _password = password;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<Response> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            // check if Username is available
            bool isNotAvailable = await _userRepository.AnyAsync(request.Username, cancellationToken);
            if (isNotAvailable)
            {
                return Response.BadRequest("Username already in use");
            }

            // TODO: server-side password policy
            
            // Generate User object
            var user = new User { Id = request.UserId ?? Guid.NewGuid(), Username=request.Username, Password=_password.HashPassword(request.Password) };

            // Save user in database
            await _userRepository.CreateAsync(user);
            
            // Commit the chages in database
            await _unitOfWork.Commit(cancellationToken);
            
            // Mapper user to dto
            UserDto userDto = _mapper.Map<UserDto>(user);
        
            return Response.Ok("User registered", userDto);
        }
        catch(Exception e)
        {
            Console.Error.WriteLine(e);
            return Response.ServerError("Internal Server Error");
        }
    }
}