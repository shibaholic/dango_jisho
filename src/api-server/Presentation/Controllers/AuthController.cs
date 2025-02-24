using Application.Mappings.EntityDtos;
using Application.Services;
using Application.UseCaseCommands.Auth;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Presentation.Extensions;
using Presentation.Utilities;

namespace Presentation.Controllers;

[ApiController]
public class AuthController: BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ICrudService<User, UserDto> _userCrud;
    public AuthController(IMediator mediadtor, ICrudService<User, UserDto> userCrud)
    {
        _mediator = mediadtor;
        _userCrud = userCrud;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);

        return this.ToActionResult(response);
    }
    
    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticationRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        if (!response.Successful) return this.ToActionResult(response);

        var jwtToken = JwtExtension.Generate(response.Data!);

        JwtExtension.SetTokensInsideCookie(jwtToken, (Guid)response.Data!.RefreshToken!, HttpContext);
        
        return Ok(response.Data.User);
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
    {
        // instead of getting refreshToken in body, get from cookies instead
        HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken);
        if(string.IsNullOrEmpty(refreshToken)) return BadRequest();
        
        // construct refreshTokenRequest
        var parseSuccess = Guid.TryParse(refreshToken, out var refreshTokenGuid);
        if(!parseSuccess) return BadRequest();
        
        var request = new RefreshTokenRequest { RefreshToken = refreshTokenGuid };
        var response = await _mediator.Send(request, cancellationToken);
        if (!response.Successful) return this.ToActionResult(response);

        var jwtToken = JwtExtension.Generate(response.Data!);
        
        JwtExtension.SetTokensInsideCookie(jwtToken, (Guid)response.Data!.RefreshToken!, HttpContext);
        
        return Ok(response.Data.User);
    }
    
    // Simply deletes the refresh token.
    // Client would delete JwtToken and Refresh Token on its own client-side.
    [HttpPost("logout")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        // instead of getting refreshToken in body, get from cookies instead
        HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken);
        if(string.IsNullOrEmpty(refreshToken)) return BadRequest();
        
        // construct refreshTokenRequest
        var parseSuccess = Guid.TryParse(refreshToken, out var refreshTokenGuid);
        if(!parseSuccess) return BadRequest();
        
        var userId = new Guid(User.FindFirst("Id")!.Value);
        
        var response = await _mediator.Send(new LogoutUserRequest(userId, refreshTokenGuid), cancellationToken);
        
        JwtExtension.SetTokensInsideCookie(null, null, HttpContext);
        
        return this.ToActionResult(response);
    }

    [HttpGet("user")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetUser()
    {
        var userId = new Guid(User.FindFirst("Id")!.Value);

        var response = await _userCrud.GetDtoByIdAsync(userId);
        
        return this.ToActionResult(response);
    }
    
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAdmin()
    {
        var userId = new Guid(User.FindFirst("Id")!.Value);

        var response = await _userCrud.GetDtoByIdAsync(userId);
        
        return this.ToActionResult(response);
    }
}