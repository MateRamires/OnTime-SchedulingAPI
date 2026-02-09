using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnTimeScheduling.Application.Security.Token;
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

        [HttpGet("me")]
        [Authorize] 
        [ProducesResponseType(typeof(ResponseUserProfileJson), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetMe([FromServices] ILoggedUser loggedUser)
        {
            var user = loggedUser.GetUser();

            var response = new ResponseUserProfileJson
            {
                Id = user.Id,
                Name = user.Name,
                CompanyId = user.CompanyId,
                Role = user.Role.ToString()
            };

            return Ok(response);
        }
    }
}
