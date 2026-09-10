using MediatR;
using Microsoft.AspNetCore.Mvc;
using Maqas.Application.DTOs;
using Maqas.Application.Features.Messages;

namespace Maqas.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MessagesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChatMessageResponseDto>>> GetMessages(
        [FromQuery] int customerId,
        [FromQuery] int tailorId,
        [FromQuery] int? sessionId = null)
    {
        var messages = await _mediator.Send(new GetChatMessagesQuery(customerId, tailorId, sessionId));
        return Ok(messages);
    }

    [HttpPost]
    public async Task<ActionResult<ChatMessageResponseDto>> SendMessage([FromBody] SendMessageDto dto)
    {
        try
        {
            var response = await _mediator.Send(new SendMessageCommand(dto));
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("mark-read/{messageId}")]
    public async Task<IActionResult> MarkAsRead(int messageId)
    {
        var success = await _mediator.Send(new MarkMessageAsReadCommand(messageId));
        if (!success) return NotFound("الرسالة غير موجودة.");
        return Ok(new { success = true, message = "تم تعليم الرسالة كمقروءة" });
    }
}
