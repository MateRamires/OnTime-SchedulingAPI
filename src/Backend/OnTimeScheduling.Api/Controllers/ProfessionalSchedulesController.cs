using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnTimeScheduling.Application.UseCases.Schedules;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Api.Controllers;

public class ProfessionalSchedulesController : OnTimeSchedulingController
{
    [HttpPost]
    [Authorize(Roles = "COMPANY_ADMIN")]
    [ProducesResponseType(typeof(ResponseRegisterScheduleJson), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Register(
        [FromServices] IRegisterScheduleUseCase useCase,
        [FromBody] RequestRegisterScheduleJson request,
        CancellationToken ct)
    {
        var response = await useCase.ExecuteAsync(request, ct);

        return Created(string.Empty, response);
    }
}
