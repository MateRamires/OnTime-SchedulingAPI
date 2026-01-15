using Microsoft.AspNetCore.Mvc;
using OnTimeScheduling.Application.UseCases.Users.CreateUser;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Api.Controllers
{
    
    public class UserController : OnTimeSchedulingController
    {
        [HttpPost]
        [ProducesResponseType(typeof(ResponseRegisteredUserJson), StatusCodes.Status201Created)]
        public async Task<IActionResult> Register([FromServices] ICreateUserUseCase useCase, [FromBody] RequestRegisterUserJson request, CancellationToken ct) 
        {
            var result = await useCase.ExecuteAsync(request, ct);

            return Created(string.Empty, result);
        }
    }
}
