using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using OnTimeScheduling.Api.RateLimiting;
using OnTimeScheduling.Application.UseCases.Users.Auth;
using OnTimeScheduling.Application.UseCases.Users.Login;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Api.Controllers;

public class AuthController : OnTimeSchedulingController
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(ResponseLoginJson), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [EnableRateLimiting(RateLimitingPolicyNames.AuthStrict)]
    public async Task<IActionResult> Login([FromServices] ILoginUseCase useCase, [FromBody] RequestLoginJson request, CancellationToken ct)
    {
        var response = await useCase.ExecuteAsync(request, ct);
        return Ok(response);
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ResponseLoginJson), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [EnableRateLimiting(RateLimitingPolicyNames.AuthStrict)]
    public async Task<IActionResult> Refresh([FromServices] IRefreshTokenUseCase useCase, [FromBody] RequestRefreshTokenJson request, CancellationToken ct)
    {
        var response = await useCase.ExecuteAsync(request, ct);
        return Ok(response);
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout([FromServices] ILogoutUseCase useCase, [FromBody] RequestLogoutJson request, CancellationToken ct)
    {
        await useCase.ExecuteAsync(request, ct);
        return NoContent();
    }

    [HttpGet("me")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    [ProducesResponseType(typeof(ResponseUserProfileJson), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMe([FromServices] IGetCurrentUserUseCase useCase, CancellationToken ct)
    {
        var response = await useCase.ExecuteAsync(ct);
        return Ok(response);
    }

}
