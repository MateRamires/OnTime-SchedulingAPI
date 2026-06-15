using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnTimeScheduling.Application.UseCases.Companies;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Api.Controllers;

public class CompaniesController : OnTimeSchedulingController
{
    [HttpPost]
    [Authorize(Roles = "SUPER_ADMIN")]
    [ProducesResponseType(typeof(ResponseRegisterCompanyJson), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Register(
        [FromServices] IRegisterCompanyUseCase useCase,
        [FromBody] RequestRegisterCompanyJson request, 
        CancellationToken ct)
    {
        var response = await useCase.ExecuteAsync(request, ct);

        return Created(string.Empty, response);
    }

    [HttpGet]
    [Authorize(Roles = "SUPER_ADMIN")]
    [ProducesResponseType(typeof(ResponsePagedResultJson<ResponseCompanyJson>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll(
        [FromServices] IGetCompaniesUseCase useCase,
        [FromQuery] RequestPaginationQuery pagination,
        [FromQuery] RecordStatus? status,
        [FromQuery] string? searchTerm,
        CancellationToken ct)
    {
        var response = await useCase.ExecuteAsync(pagination, status, searchTerm, ct);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "SUPER_ADMIN")]
    [ProducesResponseType(typeof(ResponseCompanyJson), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(
        [FromServices] IGetCompanyByIdUseCase useCase,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var response = await useCase.ExecuteAsync(id, ct);
        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SUPER_ADMIN")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(
        [FromServices] IUpdateCompanyUseCase useCase,
        [FromRoute] Guid id,
        [FromBody] RequestUpdateCompanyJson request,
        CancellationToken ct)
    {
        await useCase.ExecuteAsync(id, request, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/activate")]
    [Authorize(Roles = "SUPER_ADMIN")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Activate(
        [FromServices] IActivateCompanyUseCase useCase,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        await useCase.ExecuteAsync(id, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/inactivate")]
    [Authorize(Roles = "SUPER_ADMIN")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Inactivate(
        [FromServices] IInactivateCompanyUseCase useCase,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        await useCase.ExecuteAsync(id, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/admins")]
    [Authorize(Roles = "SUPER_ADMIN")]
    [ProducesResponseType(typeof(ResponseRegisteredUserJson), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RegisterCompanyAdmin(
        [FromServices] IRegisterCompanyAdminUseCase useCase,
        [FromRoute] Guid id,
        [FromBody] RequestRegisterCompanyAdminJson request,
        CancellationToken ct)
    {
        var response = await useCase.ExecuteAsync(id, request, ct);
        return Created(string.Empty, response);
    }
}
