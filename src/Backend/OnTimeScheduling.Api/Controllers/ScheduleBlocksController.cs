using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnTimeScheduling.Application.UseCases.ScheduleBlocks;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Api.Controllers;

public class ScheduleBlocksController : OnTimeSchedulingController
{
    [HttpPost]
    [Authorize(Roles = "COMPANY_ADMIN,ATTENDANT")]
    [ProducesResponseType(typeof(ResponseRegisterScheduleBlockJson), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromServices] IRegisterScheduleBlockUseCase useCase,
        [FromBody] RequestRegisterScheduleBlockJson request,
        CancellationToken ct)
    {
        var response = await useCase.ExecuteAsync(request, ct);
        return Created(string.Empty, response);
    }

    [HttpGet]
    [Authorize(Roles = "COMPANY_ADMIN,ATTENDANT")]
    [ProducesResponseType(typeof(ResponsePagedResultJson<ResponseScheduleBlockJson>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll(
        [FromServices] IGetScheduleBlocksUseCase useCase,
        [FromQuery] RequestGetScheduleBlocksJson request,
        CancellationToken ct)
    {
        var response = await useCase.ExecuteAsync(request, ct);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "COMPANY_ADMIN,ATTENDANT")]
    [ProducesResponseType(typeof(ResponseScheduleBlockJson), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromServices] IGetScheduleBlockByIdUseCase useCase,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var response = await useCase.ExecuteAsync(id, ct);
        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "COMPANY_ADMIN,ATTENDANT")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        [FromServices] IUpdateScheduleBlockUseCase useCase,
        [FromRoute] Guid id,
        [FromBody] RequestUpdateScheduleBlockJson request,
        CancellationToken ct)
    {
        await useCase.ExecuteAsync(id, request, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "COMPANY_ADMIN,ATTENDANT")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromServices] IDeleteScheduleBlockUseCase useCase,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        await useCase.ExecuteAsync(id, ct);
        return NoContent();
    }

}
