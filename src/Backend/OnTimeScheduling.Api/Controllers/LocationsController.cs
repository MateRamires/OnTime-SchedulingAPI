using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnTimeScheduling.Application.UseCases.Locations;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Api.Controllers;

public class LocationsController : OnTimeSchedulingController
{
    [HttpPost]
    [Authorize(Roles = "COMPANY_ADMIN")]
    [ProducesResponseType(typeof(ResponseRegisterLocationJson), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Register(
            [FromServices] IRegisterLocationUseCase useCase,
            [FromBody] RequestRegisterLocationJson request,
            CancellationToken cancellationToken)
    {
        var response = await useCase.ExecuteAsync(request, cancellationToken);

        return Created(string.Empty, response);
    }
}
