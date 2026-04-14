using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnTimeScheduling.Api.Controllers;

public class ClientsController : OnTimeSchedulingController
{
    [HttpPost]
    [Authorize(Roles = "COMPANY_ADMIN,PROVIDER")]
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
    [Authorize(Roles = "COMPANY_ADMIN,PROVIDER")]
    [ProducesResponseType(typeof(List<ResponseClientJson>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll(
        [FromServices] IGetClientsUseCase useCase,
        CancellationToken ct)
    {
        var response = await useCase.ExecuteAsync(ct);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "COMPANY_ADMIN,PROVIDER")]
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

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "COMPANY_ADMIN,PROVIDER")]
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

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "COMPANY_ADMIN")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
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
