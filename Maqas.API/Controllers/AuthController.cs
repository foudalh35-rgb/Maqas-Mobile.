using MediatR;
using Microsoft.AspNetCore.Mvc;
using Maqas.Application.DTOs;
using Maqas.Application.Features.Auth;

namespace Maqas.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        var result = await _mediator.Send(new LoginUserQuery(model.Username, model.Password, model.Role));
        if (result.Success)
        {
            return Ok(new { message = result.Message, role = result.Role, user = result.User });
        }

        return BadRequest(new { message = result.Message });
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Customer";
}
