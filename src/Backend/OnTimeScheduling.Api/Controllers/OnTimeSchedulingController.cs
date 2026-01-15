using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OnTimeScheduling.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OnTimeSchedulingController : ControllerBase
{
    protected int GetEmpresaId()
    {
        var claim = User.FindFirst("EmpresaId")?.Value;
        return string.IsNullOrEmpty(claim) ? 0 : int.Parse(claim);
    }

    protected int GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return string.IsNullOrEmpty(claim) ? 0 : int.Parse(claim);
    }
}
