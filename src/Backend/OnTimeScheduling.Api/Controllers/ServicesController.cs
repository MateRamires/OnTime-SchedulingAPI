using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnTimeScheduling.Application.UseCases.Services;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Api.Controllers;

public class ServicesController : OnTimeSchedulingController
{
    [HttpGet]
    [Authorize(Roles = "COMPANY_ADMIN,ATTENDANT")]
    public async Task<IActionResult> GetAll(
        [FromServices] IGetServicesUseCase useCase,
        [FromQuery] RecordStatus? status,
        [FromQuery] string? searchTerm,
        CancellationToken ct)
    {
        var response = await useCase.ExecuteAsync(status, searchTerm, ct);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "COMPANY_ADMIN,ATTENDANT")]
    public async Task<IActionResult> GetById(
        [FromServices] IGetServiceByIdUseCase useCase,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var response = await useCase.ExecuteAsync(id, ct);
        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "COMPANY_ADMIN")]
    public async Task<IActionResult> Update(
        [FromServices] IUpdateServiceUseCase useCase,
        [FromRoute] Guid id,
        [FromBody] RequestUpdateServiceJson request,
        CancellationToken ct)
    {
        await useCase.ExecuteAsync(id, request, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/activate")]
    [Authorize(Roles = "COMPANY_ADMIN")]
    public async Task<IActionResult> Activate(
        [FromServices] IActivateServiceUseCase useCase,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        await useCase.ExecuteAsync(id, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/inactivate")]
    [Authorize(Roles = "COMPANY_ADMIN")]
    public async Task<IActionResult> Inactivate(
        [FromServices] IInactivateServiceUseCase useCase,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        await useCase.ExecuteAsync(id, ct);
        return NoContent();
    }

    [HttpDelete("{serviceId:guid}/professionals/{professionalId:guid}")]
    [Authorize(Roles = "COMPANY_ADMIN")]
    public async Task<IActionResult> UnlinkProfessional(
        [FromServices] IUnlinkProfessionalServiceUseCase useCase,
        [FromRoute] Guid serviceId,
        [FromRoute] Guid professionalId,
        CancellationToken ct)
    {
        await useCase.ExecuteAsync(serviceId, professionalId, ct);
        return NoContent();
    }

    [HttpPost]
    [Authorize(Roles = "COMPANY_ADMIN")] 
    [ProducesResponseType(typeof(ResponseRegisterServiceJson), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)] 
    [ProducesResponseType(StatusCodes.Status403Forbidden)] 
    public async Task<IActionResult> Register(
        [FromServices] IRegisterServiceUseCase useCase,
        [FromBody] RequestRegisterServiceJson request,
        CancellationToken ct)
    {
        var response = await useCase.ExecuteAsync(request, ct);

        return Created(string.Empty, response);
    }

    [HttpPost("link")]
    [Authorize(Roles = "COMPANY_ADMIN")]
    [ProducesResponseType(typeof(ResponseLinkProfessionalServiceJson), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> LinkService(
        [FromServices] ILinkProfessionalServiceUseCase useCase,
        [FromBody] RequestLinkProfessionalServiceJson request,
        CancellationToken ct)
    {
        var response = await useCase.ExecuteAsync(request, ct);

        return Created(string.Empty, response);
    }
}
