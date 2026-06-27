using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using OnTimeScheduling.Api.RateLimiting;
using OnTimeScheduling.Application.UseCases.Appointments;
using OnTimeScheduling.Application.UseCases.Clients;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Requests.Appointments;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Communication.Responses.Appointments;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Api.Controllers;

public class ClientsController : OnTimeSchedulingController
{
    [HttpPost]
    [Authorize(Roles = "COMPANY_ADMIN,ATTENDANT")]
    [ProducesResponseType(typeof(ResponseRegisterClientJson), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Register(
        [FromServices] IRegisterClientUseCase useCase,
        [FromBody] RequestRegisterClientJson request,
        CancellationToken ct)
    {
        var response = await useCase.ExecuteAsync(request, ct);

        return Created(string.Empty, response);
    }

    [HttpGet]
    [Authorize(Roles = "COMPANY_ADMIN,ATTENDANT")]
    [ProducesResponseType(typeof(ResponsePagedResultJson<ResponseClientJson>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll(
        [FromServices] IGetClientsUseCase useCase,
        [FromQuery] RequestPaginationQuery pagination,
        [FromQuery] RecordStatus? status,
        [FromQuery] string? searchTerm,
        CancellationToken ct)
    {
        var response = await useCase.ExecuteAsync(pagination, status, searchTerm, ct);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "COMPANY_ADMIN,ATTENDANT")]
    [ProducesResponseType(typeof(ResponseClientJson), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(
        [FromServices] IGetClientByIdUseCase useCase,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var response = await useCase.ExecuteAsync(id, ct);
        return Ok(response);
    }

    [HttpGet("{id:guid}/appointments")]
    [Authorize(Roles = "COMPANY_ADMIN,ATTENDANT")]
    [ProducesResponseType(typeof(ResponsePagedResultJson<ResponseAppointmentSummaryJson>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [EnableRateLimiting(RateLimitingPolicyNames.ScheduleRead)]
    public async Task<IActionResult> GetAppointments(
        [FromServices] IGetAppointmentsUseCase useCase,
        [FromRoute] Guid id,
        [FromQuery] RequestGetAppointmentsJson request,
        CancellationToken ct)
    {
        request.ClientId = id;

        var response = await useCase.ExecuteAsync(request, ct);
        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "COMPANY_ADMIN,ATTENDANT")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(
        [FromServices] IUpdateClientUseCase useCase,
        [FromRoute] Guid id,
        [FromBody] RequestUpdateClientJson request,
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
        [FromServices] IActivateClientUseCase useCase,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        await useCase.ExecuteAsync(id, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/inactivate")]
    [Authorize(Roles = "COMPANY_ADMIN")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Inactivate(
        [FromServices] IInactivateClientUseCase useCase,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        await useCase.ExecuteAsync(id, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "COMPANY_ADMIN")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(
        [FromServices] IDeleteClientUseCase useCase,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        await useCase.ExecuteAsync(id, ct);
        return NoContent();
    }
}
