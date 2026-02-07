using Microsoft.AspNetCore.Mvc;
using OnTimeScheduling.Application.UseCases.Companies;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Api.Controllers;

public class CompaniesController : OnTimeSchedulingController
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseRegisterCompanyJson), StatusCodes.Status201Created)]
    public async Task<IActionResult> Register(
        [FromServices] IRegisterCompanyUseCase useCase,
        [FromBody] RequestRegisterCompanyJson request, 
        CancellationToken ct)
    {
        var response = await useCase.ExecuteAsync(request, ct);

        return Created(string.Empty, response);
    }
}
