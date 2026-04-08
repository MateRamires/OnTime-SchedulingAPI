using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnTimeScheduling.Application.UseCases.Appointments;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Api.Controllers;

public class AppointmentsController : OnTimeSchedulingController
{
    [HttpPost]
    [Authorize(Roles = "COMPANY_ADMIN,ATTENDANT")]
    [ProducesResponseType(typeof(ResponseRegisterAppointmentJson), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Register(
        [FromServices] IRegisterAppointmentUseCase useCase,
        [FromBody] RequestRegisterAppointmentJson request,
        CancellationToken ct)
    {
        var response = await useCase.ExecuteAsync(request, ct);

        return Created(string.Empty, response);
    }

    [HttpGet("available-slots")]
    [Authorize(Roles = "COMPANY_ADMIN,ATTENDANT")]
    [ProducesResponseType(typeof(ResponseAvailableTimeSlotsJson), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAvailableTimeSlots(
        [FromServices] IGetAvailableTimeSlotsUseCase useCase,
        [FromQuery] Guid professionalId,
        [FromQuery] Guid locationId,
        [FromQuery] Guid serviceId,
        [FromQuery] DateOnly targetDate,
        CancellationToken ct)
    {
        var request = new RequestGetAvailableTimeSlotsJson
        {
            ProfessionalId = professionalId,
            LocationId = locationId,
            ServiceId = serviceId,
            TargetDate = targetDate
        };

        var response = await useCase.ExecuteAsync(request, ct);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    [Authorize] 
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(
        [FromServices] ICancelAppointmentUseCase useCase,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        await useCase.ExecuteAsync(id, ct);

        return NoContent();
    }
}
