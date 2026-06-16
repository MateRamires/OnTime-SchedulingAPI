using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using OnTimeScheduling.Api.RateLimiting;
using OnTimeScheduling.Application.UseCases.Reports;
using OnTimeScheduling.Communication.Requests.Reports;
using OnTimeScheduling.Communication.Responses.Reports;

namespace OnTimeScheduling.Api.Controllers;

[ApiController]
[Route("reports")]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    [HttpGet("appointments-volume")]
    [Authorize(Roles = "COMPANY_ADMIN,ATTENDANT")]
    [ProducesResponseType(typeof(ResponseAppointmentsVolumeReportJson), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [EnableRateLimiting(RateLimitingPolicyNames.ScheduleRead)]
    public async Task<IActionResult> GetAppointmentsVolume(
        [FromServices] IGetAppointmentsVolumeReportUseCase useCase,
        [FromQuery] RequestAppointmentsVolumeReportJson request,
        CancellationToken ct)
    {
        var response = await useCase.ExecuteAsync(request, ct);

        return Ok(response);
    }

    [HttpGet("professional-occupancy")]
    [Authorize(Roles = "COMPANY_ADMIN,ATTENDANT")]
    [ProducesResponseType(typeof(ResponseProfessionalOccupancyReportJson), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [EnableRateLimiting(RateLimitingPolicyNames.ScheduleRead)]
    public async Task<IActionResult> GetProfessionalOccupancy(
        [FromServices] IGetProfessionalOccupancyReportUseCase useCase,
        [FromQuery] RequestProfessionalOccupancyReportJson request,
        CancellationToken ct)
    {
        var response = await useCase.ExecuteAsync(request, ct);

        return Ok(response);
    }
}
