using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using WestCoastFitness.Application.Auth;
using WestCoastFitness.Application.Auth.Dtos;

namespace WestCoastFitness.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly IValidator<LoginRequest> _validator;

    public AuthController(AuthService authService, IValidator<LoginRequest> validator)
    {
        _authService = authService;
        _validator = validator;
    }

    /// <summary>Authenticates a user and issues a JWT access token.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var result = await _authService.LoginAsync(request, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : Unauthorized(new { message = result.Error });
    }
}
