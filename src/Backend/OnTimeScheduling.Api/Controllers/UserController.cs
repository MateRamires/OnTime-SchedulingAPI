using Microsoft.AspNetCore.Mvc;
using OnTimeScheduling.Application.UseCases.Users.CreateUser;
using OnTimeScheduling.Communication.Requests;

namespace OnTimeScheduling.Api.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        //TODO: Add the expected response or the expected error to every endpoint
        [HttpPost]
        public async Task<IActionResult> Register([FromServices] ICreateUserUseCase useCase, [FromBody] RequestRegisterUserJson request, CancellationToken ct) 
        {
            var result = await useCase.ExecuteAsync(request, ct);

            return Created(string.Empty, result);
        }
    }
}
