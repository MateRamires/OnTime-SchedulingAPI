using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnTimeScheduling.Application.Security.Token;
using OnTimeScheduling.Application.UseCases.Users.CreateUser;
using OnTimeScheduling.Application.UseCases.Users.Management;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Api.Controllers;

public class UserController : OnTimeSchedulingController
{
    [HttpPost]
    [Authorize(Roles = "COMPANY_ADMIN")]
    [ProducesResponseType(typeof(ResponseRegisteredUserJson), StatusCodes.Status201Created)]
    public async Task<IActionResult> Register([FromServices] ICreateUserUseCase useCase, [FromBody] RequestRegisterUserJson request, CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(request, ct);

        return Created(string.Empty, result);
    }

    [HttpGet]
    [Authorize(Roles = "COMPANY_ADMIN")]
    [ProducesResponseType(typeof(List<ResponseUserJson>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll(
            [FromServices] IGetUsersUseCase useCase,
            [FromQuery] UserRole? role,
            [FromQuery] RecordStatus? status,
            [FromQuery] string? searchTerm,
            CancellationToken ct)
    {
        var response = await useCase.ExecuteAsync(role, status, searchTerm, ct);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "COMPANY_ADMIN")]
    [ProducesResponseType(typeof(ResponseUserJson), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(
        [FromServices] IGetUserByIdUseCase useCase,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var response = await useCase.ExecuteAsync(id, ct);
        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "COMPANY_ADMIN")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(
        [FromServices] IUpdateUserUseCase useCase,
        [FromRoute] Guid id,
        [FromBody] RequestUpdateUserJson request,
        CancellationToken ct)
    {
        await useCase.ExecuteAsync(id, request, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/activate")]
    [Authorize(Roles = "COMPANY_ADMIN")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Activate(
        [FromServices] IActivateUserUseCase useCase,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        await useCase.ExecuteAsync(id, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/inactivate")]
    [Authorize(Roles = "COMPANY_ADMIN")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Inactivate(
        [FromServices] IInactivateUserUseCase useCase,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        await useCase.ExecuteAsync(id, ct);
        return NoContent();
    }


    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ResponseUserProfileJson), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetMe([FromServices] ILoggedUser loggedUser)
    {
        var user = loggedUser.GetUser();

        var response = new ResponseUserProfileJson
        {
            Id = user.Id,
            Name = user.Name,
            CompanyId = user.CompanyId,
            Role = user.Role.ToString()
        };

        return Ok(response);
    }

    [HttpPost("admin")]
    [Authorize(Roles = "SUPER_ADMIN")]
    [ProducesResponseType(typeof(ResponseRegisteredUserJson), StatusCodes.Status201Created)]
    public async Task<IActionResult> RegisterSuperAdmin([FromServices] IRegisterSuperAdminUseCase useCase, [FromBody] RequestRegisterUserJson request, CancellationToken ct)
    {
        request.Role = (OnTimeScheduling.Communication.Enums.UserRole)(int)UserRole.SUPER_ADMIN;

        var result = await useCase.ExecuteAsync(request, ct);

        return Created(string.Empty, result);
    }
}
