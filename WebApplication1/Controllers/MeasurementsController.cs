using MediatR;
using Microsoft.AspNetCore.Mvc;
using Maqas.Application.DTOs;
using Maqas.Application.Features.Measurements;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MeasurementsController : ControllerBase
{
    private readonly IMediator _mediator;

    public MeasurementsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<MeasurementSessionResponseDto>> AddMeasurementSession([FromBody] CreateMeasurementSessionDto dto)
    {
        try
        {
            var response = await _mediator.Send(new CreateMeasurementSessionCommand(dto));
            return CreatedAtAction(nameof(GetSessionById), new { id = response.SessionId }, response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MeasurementSessionResponseDto>> GetSessionById(int id)
    {
        var session = await _mediator.Send(new GetMeasurementSessionByIdQuery(id));
        if (session == null) return NotFound("لم يتم العثور على جلسة القياس.");
        return Ok(session);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MeasurementSessionResponseDto>>> GetAllSessions()
    {
        var sessions = await _mediator.Send(new GetAllMeasurementSessionsQuery());
        return Ok(sessions);
    }

    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<MeasurementSessionResponseDto>>> GetSessionsByStatus(string status)
    {
        var sessions = await _mediator.Send(new GetMeasurementSessionsByStatusQuery(status));
        return Ok(sessions);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MeasurementSessionResponseDto>> UpdateSession(int id, [FromBody] UpdateMeasurementSessionDto dto)
    {
        try
        {
            var session = await _mediator.Send(new UpdateMeasurementSessionCommand(id, dto));
            if (session == null) return NotFound("لم يتم العثور على جلسة القياس.");
            return Ok(session);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateSessionStatus(int id, [FromBody] UpdateStatusDto dto)
    {
        var success = await _mediator.Send(new UpdateMeasurementStatusCommand(id, dto.Status));
        if (!success) return NotFound("لم يتم العثور على جلسة القياس.");
        return Ok(new { message = $"تم تحديث حالة الجلسة إلى ({dto.Status}) بنجاح." });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSession(int id)
    {
        var success = await _mediator.Send(new DeleteMeasurementSessionCommand(id));
        if (!success) return NotFound("لم يتم العثور على جلسة القياس.");
        return Ok(new { message = $"تم حذف جلسة القياس رقم {id} بنجاح." });
    }
}
