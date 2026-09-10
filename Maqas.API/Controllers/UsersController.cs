using MediatR;
using Microsoft.AspNetCore.Mvc;
using Maqas.Application.DTOs;
using Maqas.Application.Features.Users;

namespace Maqas.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
    {
        var users = await _mediator.Send(new GetUsersQuery());
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponseDto>> GetUserById(int id)
    {
        var user = await _mediator.Send(new GetUserByIdQuery(id));
        if (user == null) return NotFound("المستخدم غير موجود.");
        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserResponseDto>> CreateUser([FromBody] RegisterDto dto)
    {
        var user = await _mediator.Send(new CreateUserCommand(dto));
        return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserResponseDto>> UpdateUser(int id, [FromBody] UpdateUserDto dto)
    {
        var user = await _mediator.Send(new UpdateUserCommand(id, dto));
        if (user == null) return NotFound("المستخدم غير موجود.");
        return Ok(user);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<string>> DeleteUser(int id)
    {
        var success = await _mediator.Send(new DeleteUserCommand(id));
        if (!success) return NotFound("المستخدم غير موجود.");
        return Ok("تم حذف المستخدم بنجاح.");
    }
}
