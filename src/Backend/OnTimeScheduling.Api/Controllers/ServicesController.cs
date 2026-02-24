using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnTimeScheduling.Application.UseCases.Services;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Api.Controllers;

public class ServicesController : OnTimeSchedulingController
{
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
