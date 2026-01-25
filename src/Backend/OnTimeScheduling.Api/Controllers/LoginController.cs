using Microsoft.AspNetCore.Mvc;
using OnTimeScheduling.Application.UseCases.Users.Login;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Api.Controllers
{
    public class LoginController : OnTimeSchedulingController
    {
        [HttpPost]
        [ProducesResponseType(typeof(ResponseLoginJson), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromServices] ILoginUseCase useCase, [FromBody] RequestLoginJson request, CancellationToken ct)
        {
            var response = await useCase.ExecuteAsync(request, ct);
            return Ok(response);
        }
        //TODO: finish login process
    }
}
